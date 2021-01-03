using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Queries;

namespace Haus.Core.Rooms.Queries
{
    public record GetRoomsQuery : IQuery<ListResult<RoomModel>>;

    public class GetRoomsQueryHandler : IQueryHandler<GetRoomsQuery, ListResult<RoomModel>>
    {
        private readonly HausDbContext _context;

        public GetRoomsQueryHandler(HausDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<RoomModel>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            return await _context.QueryAll<RoomEntity>()
                .Select(RoomEntity.ToModelExpression)
                .ToListResultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}