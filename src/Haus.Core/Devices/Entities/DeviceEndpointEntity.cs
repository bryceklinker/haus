using System.Collections.Generic;
using Haus.Core.Models.Devices;

namespace Haus.Core.Devices.Entities;

public record DeviceEndpointEntity(byte EndpointId = 0, IReadOnlyList<ushort>? InClusters = null)
{
    public long Id { get; set; }
    public required DeviceEntity Device { get; set; }
    public IReadOnlyList<ushort> InClusters { get; init; } = InClusters ?? [];

    public DeviceEndpointModel ToModel()
    {
        return new DeviceEndpointModel(EndpointId, InClusters);
    }
}
