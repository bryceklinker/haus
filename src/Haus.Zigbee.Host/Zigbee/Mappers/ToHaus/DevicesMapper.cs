using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Zigbee;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;

public class DevicesMapper(IZigbeeCoordinator coordinator, DeviceJoinedMapper deviceJoinedMapper)
{
    public async Task<IEnumerable<DeviceDiscoveredEvent>> MapAsync(
        IReadOnlyList<ZigbeeDevice> devices,
        CancellationToken token
    )
    {
        var events = new List<DeviceDiscoveredEvent>();
        foreach (var device in devices)
        {
            var discovered = await CreateDeviceDiscoveredEvent(device, token);
            if (discovered != null)
                events.Add(discovered);
        }

        return events;
    }

    // Mirrors DeviceBackfillService.BackfillDeviceAsync: classify from the Basic cluster's
    // vendor/model, and skip publishing entirely when that resolves to Unknown --
    // DeviceEntity.UpdateFromDiscoveredDevice unconditionally overwrites DeviceType, so publishing
    // Unknown here would clobber an already-known device's classification on every discovery sync.
    private async Task<DeviceDiscoveredEvent?> CreateDeviceDiscoveredEvent(ZigbeeDevice device, CancellationToken token)
    {
        var info = await coordinator.ReadDeviceInfoAsync(device.IeeeAddress, token);
        if (info == null)
            return null;

        var joined = new ZigbeeDeviceJoined(
            device.IeeeAddress,
            device.NetworkAddress,
            device.Endpoints,
            info.ManufacturerName,
            info.ModelIdentifier
        );
        var discovered = deviceJoinedMapper.Map(joined);
        return discovered.DeviceType == DeviceType.Unknown ? null : discovered;
    }
}
