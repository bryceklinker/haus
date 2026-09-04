using System.Linq;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices.Events;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;

public class DeviceJoinedMapper(IDeviceTypeResolver deviceTypeResolver)
{
    private const ushort OnOffClusterId = 0x0006;
    private const ushort LevelClusterId = 0x0008;
    private const ushort ColorControlClusterId = 0x0300;

    public DeviceDiscoveredEvent Map(ZigbeeDeviceJoined joined)
    {
        return new DeviceDiscoveredEvent(
            ExternalIdConverter.ToExternalId(joined.IeeeAddress),
            deviceTypeResolver.Resolve(joined.ManufacturerName, joined.ModelIdentifier),
            CreateMetadata(joined),
            NetworkAddress: joined.NetworkAddress,
            EndpointId: SelectEndpointId(joined)
        );
    }

    private static byte? SelectEndpointId(ZigbeeDeviceJoined joined)
    {
        return FindEndpointWithCluster(joined, OnOffClusterId)?.EndpointId
            ?? FindEndpointWithCluster(joined, LevelClusterId)?.EndpointId
            ?? FindEndpointWithCluster(joined, ColorControlClusterId)?.EndpointId
            ?? joined.Endpoints.FirstOrDefault()?.EndpointId;
    }

    private static ZigbeeEndpoint? FindEndpointWithCluster(ZigbeeDeviceJoined joined, ushort clusterId)
    {
        return joined.Endpoints.FirstOrDefault(endpoint => endpoint.InClusters.Contains(clusterId));
    }

    // Registering the device's network address before mapping it is a step every caller needs --
    // a freshly-joined device and an already-known device being backfilled both go through this
    // same register-then-map sequence before deciding what (if anything) to publish.
    public DeviceDiscoveredEvent RegisterAndMap(DeviceAddressRegistry addressRegistry, ZigbeeDeviceJoined joined)
    {
        addressRegistry.Register(joined.NetworkAddress, ExternalIdConverter.ToExternalId(joined.IeeeAddress));
        return Map(joined);
    }

    private static MetadataModel[] CreateMetadata(ZigbeeDeviceJoined joined)
    {
        return
        [
            new MetadataModel("vendor", joined.ManufacturerName),
            new MetadataModel("model", joined.ModelIdentifier),
        ];
    }
}
