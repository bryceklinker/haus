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

    // Registering the device's network address before mapping it is a step every caller needs --
    // a freshly-joined device and an already-known device being backfilled both go through this
    // same register-then-map sequence before deciding what (if anything) to publish.
    public DeviceDiscoveredEvent RegisterAndMap(DeviceAddressRegistry addressRegistry, ZigbeeDeviceJoined joined)
    {
        addressRegistry.Register(joined.NetworkAddress, ExternalIdMap.ToExternalId(joined.IeeeAddress));
        return Map(joined);
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
