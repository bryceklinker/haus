using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Queries
{
    public record GetRoomByIdQuery(long Id) : GetByIdQuery<RoomModel>(Id);

    internal class GetRoomByIdQueryHandler : IQueryHandler<GetRoomByIdQuery, RoomModel>
    {
        private readonly HausDbContext _context;

        public GetRoomByIdQueryHandler(HausDbContext context)
        {
            _context = context;
        }

        public async Task<RoomModel> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.QueryAll<RoomEntity>()
                .Select(RoomEntity.ToModelExpression)
                .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}