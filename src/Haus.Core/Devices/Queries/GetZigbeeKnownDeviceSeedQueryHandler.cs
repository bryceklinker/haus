using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Zigbee.Events;
using Haus.Cqrs.Queries;

namespace Haus.Core.Devices.Queries;

public record GetZigbeeKnownDeviceSeedQuery : IQuery<ListResult<ZigbeeKnownDeviceSeedEntry>>;

internal class GetZigbeeKnownDeviceSeedQueryHandler(HausDbContext context)
    : IQueryHandler<GetZigbeeKnownDeviceSeedQuery, ListResult<ZigbeeKnownDeviceSeedEntry>>
{
    private static readonly Expression<Func<DeviceEntity, ZigbeeKnownDeviceSeedEntry>> ToSeedEntryExpression =
        d => new ZigbeeKnownDeviceSeedEntry(
            d.ExternalId,
            d.NetworkAddress!.Value,
            d.Endpoints.Select(e => new DeviceEndpointModel(e.EndpointId, e.InClusters)).ToArray(),
            d.IsSleepy
        );

    public async Task<ListResult<ZigbeeKnownDeviceSeedEntry>> Handle(
        GetZigbeeKnownDeviceSeedQuery request,
        CancellationToken cancellationToken = default
    )
    {
        return await context
            .QueryAll<DeviceEntity>()
            .Where(d => d.NetworkAddress != null)
            .Select(ToSeedEntryExpression)
            .ToListResultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
