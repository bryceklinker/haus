using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Queries;

namespace Haus.Core.Rooms.Queries;

public record GetRoomsQuery : IQuery<ListResult<RoomModel>>;

public class GetRoomsQueryHandler(HausDbContext context) : IQueryHandler<GetRoomsQuery, ListResult<RoomModel>>
{
    public async Task<ListResult<RoomModel>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        return await context
            .QueryAll<RoomEntity>()
            .Select(RoomEntity.ToModelExpression)
            .ToListResultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
