using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Queries
{
    public class GetRoomByIdQuery : GetByIdQuery<RoomModel>
    {
        public GetRoomByIdQuery(long id) 
            : base(id)
        {
        }
    }

    internal class GetRoomByIdQueryHandler : IQueryHandler<GetRoomByIdQuery, RoomModel>
    {
        private readonly HausDbContext _context;
        private readonly IMapper _mapper;

        public GetRoomByIdQueryHandler(HausDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<RoomModel> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.GetAllReadOnly<RoomEntity>()
                .ProjectTo<RoomModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}