using System.Collections.Generic;

namespace Haus.Core.Models.Devices;

public record DeviceEndpointModel(byte EndpointId = 0, IReadOnlyList<ushort>? InClusters = null)
{
    public IReadOnlyList<ushort> InClusters { get; init; } = InClusters ?? [];
}
