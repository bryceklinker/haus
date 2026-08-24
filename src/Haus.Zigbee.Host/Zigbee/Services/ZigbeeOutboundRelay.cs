using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee.Events;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Polly;
using Polly.Retry;

namespace Haus.Zigbee.Host.Zigbee.Services;

public class ZigbeeOutboundRelay(
    IZigbeeCoordinator coordinator,
    HausDiscoveryToZigbeeMapper discoveryMapper,
    HausLightingToZigbeeMapper lightingMapper,
    DevicesMapper devicesMapper,
    DeviceAddressRegistry addressRegistry,
    IHausMqttClientFactory mqttClientFactory,
    ILogger<ZigbeeOutboundRelay> logger
)
{
    // The Host doesn't yet track a per-device endpoint from ZDP discovery, and endpoint 1 is what
    // the overwhelming majority of commercial Zigbee lighting devices expose.
    private const byte DefaultDestinationEndpoint = 0x01;

    // Per the Zigbee APS spec, a confirm status of 0x00 is APS_SUCCESS; anything else is a delivery
    // failure reported back from the stack for that specific request.
    private const byte SuccessConfirmStatus = 0x00;

    public async Task HandleCommandAsync(MqttApplicationMessage message, CancellationToken token)
    {
        var command = HausJsonSerializer.Deserialize<HausCommand>(message.PayloadSegment);
        if (command?.Type == null)
        {
            logger.LogWarning("Received an unparseable Haus command on {@Topic}", message.Topic);
            return;
        }

        logger.LogInformation("Received Haus command {@Type}", command.Type);
        if (discoveryMapper.IsSupported(command.Type))
            await HandleDiscoveryAsync(command.Type, token);
        else if (lightingMapper.IsSupported(command.Type))
            await HandleLightingAsync(message, token);
        else
            logger.LogInformation("No handler for Haus command {@Type}, ignoring", command.Type);
    }

    private async Task HandleDiscoveryAsync(string commandType, CancellationToken token)
    {
        var intent = discoveryMapper.Map(commandType);
        if (intent.Type == ZigbeeDiscoveryIntentType.SetPermitJoin)
        {
            logger.LogInformation("Setting permit-join to {@Enabled}", intent.PermitJoinEnabled);
            await coordinator.SetPermitJoinAsync(intent.PermitJoinEnabled, token);
            return;
        }

        await SyncDevicesAsync(token);
    }

    private async Task SyncDevicesAsync(CancellationToken token)
    {
        var devices = await coordinator.GetDevicesAsync(token);
        logger.LogInformation("Syncing {@Count} known device(s) to Haus", devices.Count);
        foreach (var device in devices)
            addressRegistry.Register(device.NetworkAddress, ExternalIdConverter.ToExternalId(device.IeeeAddress));

        var hausMqttClient = await mqttClientFactory.CreateClient();
        foreach (var discovered in devicesMapper.Map(devices))
            await hausMqttClient.PublishHausEventAsync(discovered);
    }

    private async Task HandleLightingAsync(MqttApplicationMessage message, CancellationToken token)
    {
        var command = HausJsonSerializer.Deserialize<HausCommand<DeviceLightingChangedEvent>>(message.PayloadSegment);
        if (command?.Payload?.Lighting == null)
        {
            logger.LogWarning("Received an unparseable lighting command on {@Topic}", message.Topic);
            return;
        }

        var device = command.Payload.Device;
        var resolvedNetworkAddress = device.NetworkAddress ?? await ResolveNetworkAddressAsync(device, token);
        if (resolvedNetworkAddress is not { } networkAddress)
        {
            await HandleMissingNetworkAddressAsync(device);
            return;
        }

        var destination = ApsDestination.Nwk(networkAddress, DefaultDestinationEndpoint);
        var requests = lightingMapper.Map(destination, command.Payload.Lighting);
        foreach (var request in requests)
        {
            logger.LogInformation(
                "Sending ZCL command {@ClusterId}/{@CommandId} to {@ExternalId}",
                request.ClusterId,
                request.CommandId,
                device.ExternalId
            );
            var confirm = await SendWithApsAckRetryAsync(request, device.ExternalId, token);
            if (confirm.ConfirmStatus != SuccessConfirmStatus)
                logger.LogWarning(
                    "Zigbee command {@ClusterId}/{@CommandId} to {@ExternalId} failed with APS confirm status {@ConfirmStatus}",
                    request.ClusterId,
                    request.CommandId,
                    device.ExternalId,
                    confirm.ConfirmStatus
                );
        }
    }

    // A previously-paired device whose NetworkAddress went stale over a Host restart can still be
    // reached by broadcasting for its current short address before giving up. An ExternalId that
    // does not parse to an IEEE address, or a broadcast nobody answers, both resolve to null and
    // fall through to the same drop-and-log path as before.
    private async Task<ushort?> ResolveNetworkAddressAsync(DeviceModel device, CancellationToken token)
    {
        if (!ExternalIdConverter.TryParseAddress(device.ExternalId, out var ieeeAddress))
            return null;

        if (await coordinator.ResolveNetworkAddressAsync(ieeeAddress, token) is not { } networkAddress)
            return null;

        addressRegistry.Register(networkAddress, device.ExternalId);

        // DeviceEntity.UpdateFromDiscoveredDevice unconditionally overwrites DeviceType, so the
        // event must carry this command's already-classified DeviceType/Metadata -- publishing
        // DeviceType.Unknown here would silently reclassify a known light back to Unknown on every
        // stale-address command after a restart.
        var hausMqttClient = await mqttClientFactory.CreateClient();
        await hausMqttClient.PublishHausEventAsync(
            new DeviceDiscoveredEvent(device.ExternalId, device.DeviceType, device.Metadata, networkAddress)
        );
        return networkAddress;
    }

    private async Task HandleMissingNetworkAddressAsync(DeviceModel device)
    {
        const string reason = "no known network address";
        logger.LogWarning("Cannot send a lighting command to {@ExternalId}: {@Reason}", device.ExternalId, reason);
        var hausMqttClient = await mqttClientFactory.CreateClient();
        await hausMqttClient.PublishHausEventAsync(new ZigbeeCommandDroppedEvent(device.ExternalId, reason));
    }

    // Mirrors zigbee-herdsman's deCONZ adapter: on a failed confirm, retry exactly once at the
    // same NWK address (no re-resolve, no mode switch) escalated to an APS-ACK request, then
    // surface whatever the last attempt reported. `token` already bounds "time remains" -- if it's
    // been canceled the pipeline won't retry. The retry flag is captured per call, not shared
    // state, since concurrent lighting commands each build and run their own pipeline instance;
    // that construction is negligible next to the tens-of-seconds a real Zigbee round trip takes.
    private async Task<ApsDataConfirm> SendWithApsAckRetryAsync(
        ZigbeeCommandRequest request,
        string externalId,
        CancellationToken token
    )
    {
        var isRetryAttempt = false;
        var pipeline = new ResiliencePipelineBuilder<ApsDataConfirm>()
            .AddRetry(
                new RetryStrategyOptions<ApsDataConfirm>
                {
                    MaxRetryAttempts = 1,
                    Delay = TimeSpan.Zero,
                    ShouldHandle = new PredicateBuilder<ApsDataConfirm>().HandleResult(confirm =>
                        confirm.ConfirmStatus != SuccessConfirmStatus
                    ),
                    OnRetry = args =>
                    {
                        isRetryAttempt = true;
                        logger.LogWarning(
                            "Retrying Zigbee command {@ClusterId}/{@CommandId} to {@ExternalId} with APS-ACK after confirm status {@ConfirmStatus}",
                            request.ClusterId,
                            request.CommandId,
                            externalId,
                            args.Outcome.Result?.ConfirmStatus
                        );
                        return default;
                    },
                }
            )
            .Build();

        return await pipeline.ExecuteAsync(
            ct => new ValueTask<ApsDataConfirm>(
                coordinator.SendCommandAsync(request with { RequestApsAck = isRetryAttempt }, ct)
            ),
            token
        );
    }
}
