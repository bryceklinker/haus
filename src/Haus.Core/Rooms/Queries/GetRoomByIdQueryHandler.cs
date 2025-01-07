using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Queries;

public record GetRoomByIdQuery(long Id) : GetByIdQuery<RoomModel>(Id);

internal class GetRoomByIdQueryHandler(HausDbContext context) : IQueryHandler<GetRoomByIdQuery, RoomModel>
{
    public async Task<RoomModel> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.QueryAll<RoomEntity>()
            .Where(r => r.Id == request.Id)
            .Select(RoomEntity.ToModelExpression)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}