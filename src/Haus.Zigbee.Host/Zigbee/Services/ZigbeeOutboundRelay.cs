using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Haus.Zigbee.Serial.Frames;
using Microsoft.Extensions.Logging;
using MQTTnet;

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
        if (device.NetworkAddress is not { } networkAddress)
        {
            logger.LogWarning(
                "Cannot send a lighting command to {@ExternalId}: no known network address",
                device.ExternalId
            );
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
            var confirm = await coordinator.SendCommandAsync(request, token);
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
}
