using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Devices.Events
{
    internal class OccupancyChangedEventHandler : IEventHandler<RoutableEvent<OccupancyChangedModel>>
    {
        private readonly HausDbContext _context;
        private readonly IDomainEventBus _domainEventBus;

        public OccupancyChangedEventHandler(HausDbContext context, IDomainEventBus domainEventBus)
        {
            _context = context;
            _domainEventBus = domainEventBus;
        }

        public async Task Handle(RoutableEvent<OccupancyChangedModel> notification, CancellationToken cancellationToken)
        {
            var deviceExternalId = notification.Payload.DeviceId;
            var room = await GetRoomByDeviceExternalId(deviceExternalId, cancellationToken).ConfigureAwait(false);
            if (room == null)
                return;

            if (notification.Payload.Occupancy)
                room.TurnOn(_domainEventBus);
            else
                room.TurnOff(_domainEventBus);

            await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task<RoomEntity> GetRoomByDeviceExternalId(string externalId, CancellationToken token)
        {
            return await _context.GetAll<DeviceEntity>()
                .Where(d => d.ExternalId == externalId)
                .Select(d => d.Room)
                .SingleOrDefaultAsync(token)
                .ConfigureAwait(false);
        }
    }
}