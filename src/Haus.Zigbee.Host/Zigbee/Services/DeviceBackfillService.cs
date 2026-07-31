using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Models;
using Microsoft.Extensions.Logging;

namespace Haus.Zigbee.Host.Zigbee.Services;

// Device-interview resolves vendor/model for a device the moment it joins, but a device that was
// already paired before this coordinator connected was never interviewed by it -- this re-reads
// each already-known device's Basic cluster on connect and re-publishes DeviceDiscoveredEvent once
// resolved, the same way a fresh join would.
public class DeviceBackfillService(
    IZigbeeCoordinator coordinator,
    DeviceAddressRegistry addressRegistry,
    DeviceJoinedMapper deviceJoinedMapper,
    IHausMqttClientFactory mqttClientFactory,
    ILogger<DeviceBackfillService> logger
)
{
    public async Task BackfillAsync(CancellationToken token)
    {
        var devices = await coordinator.GetDevicesAsync(token);
        foreach (var device in devices)
            await BackfillDeviceAsync(device, token);
    }

    private async Task BackfillDeviceAsync(ZigbeeDevice device, CancellationToken token)
    {
        try
        {
            var info = await coordinator.ReadDeviceInfoAsync(device.IeeeAddress, token);
            if (info == null)
                return;

            addressRegistry.Register(device.NetworkAddress, ExternalIdMap.ToExternalId(device.IeeeAddress));

            var joined = new ZigbeeDeviceJoined(
                device.IeeeAddress,
                device.NetworkAddress,
                device.Endpoints,
                info.ManufacturerName,
                info.ModelIdentifier
            );
            var discovered = deviceJoinedMapper.Map(joined);
            if (discovered.DeviceType == DeviceType.Unknown)
                return;

            var mqttClient = await mqttClientFactory.CreateClient();
            await mqttClient.PublishHausEventAsync(discovered);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to backfill device info for {@Address}", device.IeeeAddress);
        }
    }
}
