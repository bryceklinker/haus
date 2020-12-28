using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs;
using Haus.Cqrs.DomainEvents;

namespace Haus.Core.Rooms.DomainEvents
{
    public class RoomLightingChangedDomainEvent : IDomainEvent
    {
        public RoomEntity Room { get; }
        public Lighting Lighting { get; }

        public RoomLightingChangedDomainEvent(RoomEntity room, Lighting lighting)
        {
            Room = room;
            Lighting = lighting;
        }
    }
    
    internal class RoomLightingChangedDomainEventHandler : IDomainEventHandler<RoomLightingChangedDomainEvent>
    {
        private readonly IHausBus _hausBus;
        private readonly IMapper _mapper;

        public RoomLightingChangedDomainEventHandler(IHausBus hausBus, IMapper mapper)
        {
            _hausBus = hausBus;
            _mapper = mapper;
        }

        public Task Handle(RoomLightingChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var room = _mapper.Map<RoomModel>(notification.Room);
            var lighting = _mapper.Map<LightingModel>(notification.Lighting);
            var @event = new RoomLightingChangedEvent(room, lighting);
            return _hausBus.PublishAsync(RoutableCommand.FromEvent(@event), cancellationToken);
        }
    }
}