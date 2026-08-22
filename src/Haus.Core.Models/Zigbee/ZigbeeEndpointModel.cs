using System.Collections.Generic;

namespace Haus.Core.Models.Zigbee;

public record ZigbeeEndpointModel(byte EndpointId, IReadOnlyList<ushort> InClusters, IReadOnlyList<ushort> OutClusters);
