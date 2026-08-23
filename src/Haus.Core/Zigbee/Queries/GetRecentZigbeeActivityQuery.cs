using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Queries;

namespace Haus.Core.Zigbee.Queries;

public record GetRecentZigbeeActivityQuery : IQuery<ListResult<ZigbeeActivityEntryModel>>;

public class GetRecentZigbeeActivityQueryHandler(IZigbeeState state)
    : IQueryHandler<GetRecentZigbeeActivityQuery, ListResult<ZigbeeActivityEntryModel>>
{
    public Task<ListResult<ZigbeeActivityEntryModel>> Handle(
        GetRecentZigbeeActivityQuery request,
        CancellationToken cancellationToken
    )
    {
        var mostRecentFirst = state.RecentActivity.Reverse().ToArray();
        return Task.FromResult(new ListResult<ZigbeeActivityEntryModel>(mostRecentFirst));
    }
}
