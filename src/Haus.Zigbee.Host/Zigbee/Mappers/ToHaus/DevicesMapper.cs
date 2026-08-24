using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;

public class DevicesMapper
{
    public IEnumerable<DeviceDiscoveredEvent> Map(IReadOnlyList<ZigbeeDevice> devices)
    {
        return devices.Select(CreateDeviceDiscoveredEvent);
    }

    private static DeviceDiscoveredEvent CreateDeviceDiscoveredEvent(ZigbeeDevice device)
    {
        return new DeviceDiscoveredEvent(
            ExternalIdConverter.ToExternalId(device.IeeeAddress),
            DeviceType.Unknown,
            NetworkAddress: device.NetworkAddress
        );
    }
}
