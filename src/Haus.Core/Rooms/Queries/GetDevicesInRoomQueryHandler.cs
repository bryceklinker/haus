using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Queries;

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

        public GetDevicesInRoomQueryHandler(HausDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<DeviceModel>> Handle(GetDevicesInRoomQuery request, CancellationToken cancellationToken)
        {
            if (await _context.IsMissingAsync<RoomEntity>(request.RoomId).ConfigureAwait(false))
                return null;
            
            return await _context.GetAllReadOnly<DeviceEntity>()
                .Where(d => d.Room.Id == request.RoomId)
                .Select(DeviceEntity.ToModelExpression)
                .ToListResultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}