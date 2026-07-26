using System.Collections.Generic;

namespace Haus.Zigbee;

public record ZigbeeEndpoint(
    byte EndpointId,
    ushort ProfileId,
    ushort DeviceId,
    IReadOnlyList<ushort> InClusters,
    IReadOnlyList<ushort> OutClusters
);
