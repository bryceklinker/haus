using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Mqtt.Client;
using Haus.Zigbee.Coordinator;
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
    IHausMqttClient hausMqttClient,
    ILogger<ZigbeeOutboundRelay> logger
)
{
    // The Host doesn't yet track a per-device endpoint from ZDP discovery, and endpoint 1 is what
    // the overwhelming majority of commercial Zigbee lighting devices expose.
    private const byte DefaultDestinationEndpoint = 0x01;

    public async Task HandleCommandAsync(MqttApplicationMessage message, CancellationToken token)
    {
        var command = HausJsonSerializer.Deserialize<HausCommand>(message.PayloadSegment);
        if (command?.Type == null)
            return;

        if (discoveryMapper.IsSupported(command.Type))
            await HandleDiscoveryAsync(command.Type, token);
        else if (lightingMapper.IsSupported(command.Type))
            await HandleLightingAsync(message, token);
    }

    private async Task HandleDiscoveryAsync(string commandType, CancellationToken token)
    {
        var intent = discoveryMapper.Map(commandType);
        if (intent.Type == ZigbeeDiscoveryIntentType.SetPermitJoin)
        {
            await coordinator.SetPermitJoinAsync(intent.PermitJoinEnabled, token);
            return;
        }

        await SyncDevicesAsync(token);
    }

    private async Task SyncDevicesAsync(CancellationToken token)
    {
        var devices = await coordinator.GetDevicesAsync(token);
        foreach (var device in devices)
            addressRegistry.Register(device.NetworkAddress, ExternalIdMap.ToExternalId(device.IeeeAddress));

        foreach (var discovered in devicesMapper.Map(devices))
            await hausMqttClient.PublishHausEventAsync(discovered);
    }

    private async Task HandleLightingAsync(MqttApplicationMessage message, CancellationToken token)
    {
        var command = HausJsonSerializer.Deserialize<HausCommand<DeviceLightingChangedEvent>>(message.PayloadSegment);
        if (command?.Payload?.Lighting == null)
            return;

        var externalId = command.Payload.Device.ExternalId;
        if (!ExternalIdMap.TryParseAddress(externalId, out var address))
        {
            logger.LogWarning("Cannot resolve a Zigbee address for ExternalId {@ExternalId}", externalId);
            return;
        }

        var destination = ApsDestination.Ieee(address, DefaultDestinationEndpoint);
        foreach (var request in lightingMapper.Map(destination, command.Payload.Lighting))
            await coordinator.SendCommandAsync(request, token);
    }
}
