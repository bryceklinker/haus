using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;

namespace Haus.Core.Rooms.Queries
{
    public class GetRoomsQuery : IQuery<ListResult<RoomModel>>
    {
    }

    public class GetRoomsQueryHandler : IQueryHandler<GetRoomsQuery, ListResult<RoomModel>>
    {
        private readonly HausDbContext _context;
        private readonly IMapper _mapper;

        public GetRoomsQueryHandler(HausDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ListResult<RoomModel>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            return await _context.GetAllReadOnly<RoomEntity>()
                .ProjectTo<RoomModel>(_mapper.ConfigurationProvider)
                .ToListResultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}