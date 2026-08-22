using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Zigbee;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Queries;

namespace Haus.Core.Zigbee.Queries;

public record GetZigbeeConnectionStatusQuery : IQuery<ZigbeeConnectionStatusModel>;

public class GetZigbeeConnectionStatusQueryHandler(IZigbeeState state)
    : IQueryHandler<GetZigbeeConnectionStatusQuery, ZigbeeConnectionStatusModel>
{
    public Task<ZigbeeConnectionStatusModel> Handle(
        GetZigbeeConnectionStatusQuery request,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(state.ConnectionStatus);
    }
}
