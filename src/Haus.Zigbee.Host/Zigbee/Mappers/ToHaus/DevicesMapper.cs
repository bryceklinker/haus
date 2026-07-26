using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;

public class DevicesMapper
{
    public IEnumerable<DeviceDiscoveredEvent> Map(IReadOnlyList<Haus.Zigbee.ZigbeeDevice> devices)
    {
        return devices.Select(CreateDeviceDiscoveredEvent);
    }

    private static DeviceDiscoveredEvent CreateDeviceDiscoveredEvent(Haus.Zigbee.ZigbeeDevice device)
    {
        return new DeviceDiscoveredEvent(
            ExternalIdMap.ToExternalId(device.IeeeAddress),
            DeviceType.Unknown,
            CreateMetadata(device)
        );
    }

    private static MetadataModel[] CreateMetadata(Haus.Zigbee.ZigbeeDevice device)
    {
        return [new MetadataModel("network_address", device.NetworkAddress.ToString())];
    }
}
