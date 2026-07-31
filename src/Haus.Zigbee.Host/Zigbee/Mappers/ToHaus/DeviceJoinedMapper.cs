using Haus.Core.Models.Common;
using Haus.Core.Models.Devices.Events;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;

public class DeviceJoinedMapper(IDeviceTypeResolver deviceTypeResolver)
{
    public DeviceDiscoveredEvent Map(ZigbeeDeviceJoined joined)
    {
        return new DeviceDiscoveredEvent(
            ExternalIdMap.ToExternalId(joined.IeeeAddress),
            deviceTypeResolver.Resolve(joined.ManufacturerName, joined.ModelIdentifier),
            CreateMetadata(joined)
        );
    }

    private static MetadataModel[] CreateMetadata(ZigbeeDeviceJoined joined)
    {
        return
        [
            new MetadataModel("vendor", joined.ManufacturerName),
            new MetadataModel("model", joined.ModelIdentifier),
            new MetadataModel("network_address", joined.NetworkAddress.ToString()),
        ];
    }
}
