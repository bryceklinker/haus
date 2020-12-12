using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Rooms.Entities;

namespace Haus.Core.Rooms.Queries
{
    public class GetDevicesInRoomQuery : IQuery<ListResult<DeviceModel>>
    {
        public long RoomId { get; }

        public GetDevicesInRoomQuery(long roomId)
        {
            RoomId = roomId;
        }
    }

    public class GetDevicesInRoomQueryHandler : IQueryHandler<GetDevicesInRoomQuery, ListResult<DeviceModel>>
    {
        private readonly HausDbContext _context;
        private readonly IMapper _mapper;

        public GetDevicesInRoomQueryHandler(HausDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ListResult<DeviceModel>> Handle(GetDevicesInRoomQuery request, CancellationToken cancellationToken)
        {
            if (await _context.IsMissingAsync<RoomEntity>(request.RoomId).ConfigureAwait(false))
                return null;
            
            return await _context.GetAllReadOnly<DeviceEntity>()
                .Where(d => d.Room.Id == request.RoomId)
                .ProjectTo<DeviceModel>(_mapper.ConfigurationProvider)
                .ToListResultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}