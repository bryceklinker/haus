using System.Collections.Generic;

namespace Haus.Zigbee.Models;

public record ZigbeeEndpoint(
    byte EndpointId,
    ushort ProfileId,
    ushort DeviceId,
    IReadOnlyList<ushort> InClusters,
    IReadOnlyList<ushort> OutClusters
);
