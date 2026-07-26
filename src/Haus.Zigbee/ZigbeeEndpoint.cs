using System.Collections.Generic;

namespace Haus.Zigbee;

public sealed record ZigbeeEndpoint(
    byte EndpointId,
    ushort ProfileId,
    ushort DeviceId,
    IReadOnlyList<ushort> InClusters,
    IReadOnlyList<ushort> OutClusters
);
