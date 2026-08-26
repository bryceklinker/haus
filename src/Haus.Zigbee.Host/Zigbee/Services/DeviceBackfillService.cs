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
            // A resolve refreshes the address in case it changed since this device was last seen;
            // no answer falls back to the address already on record rather than abandoning backfill.
            var networkAddress =
                await coordinator.ResolveNetworkAddressAsync(device.IeeeAddress, token) ?? device.NetworkAddress;

            var info = await coordinator.ReadDeviceInfoAsync(device.IeeeAddress, token);
            if (info == null)
                return;

            var joined = new ZigbeeDeviceJoined(
                device.IeeeAddress,
                networkAddress,
                device.Endpoints,
                info.ManufacturerName,
                info.ModelIdentifier
            );
            var discovered = deviceJoinedMapper.RegisterAndMap(addressRegistry, joined);
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
