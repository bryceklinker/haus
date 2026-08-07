using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Common;
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
            ExternalIdMap.ToExternalId(device.IeeeAddress),
            DeviceType.Unknown,
            CreateMetadata(device)
        );
    }

    private static MetadataModel[] CreateMetadata(ZigbeeDevice device)
    {
        return [new MetadataModel("network_address", device.NetworkAddress.ToString())];
    }
}
