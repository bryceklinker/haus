using System.Collections.Generic;

namespace Haus.Zigbee.Models.Diagnostics;

public record ZigbeeEndpointModel(
    byte EndpointId,
    ushort ProfileId,
    ushort DeviceId,
    IReadOnlyList<ushort> InClusters,
    IReadOnlyList<ushort> OutClusters
);
