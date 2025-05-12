using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs;
using Haus.Cqrs.DomainEvents;

namespace Haus.Core.Rooms.DomainEvents;

public record RoomLightingChangedDomainEvent(RoomEntity Room, LightingEntity Lighting) : IDomainEvent
{
    public RoomModel RoomModel => Room.ToModel();
    public LightingModel LightingModel => Lighting.ToModel();
}

internal class RoomLightingChangedDomainEventHandler(IHausBus hausBus)
    : IDomainEventHandler<RoomLightingChangedDomainEvent>
{
    public Task Handle(RoomLightingChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new RoomLightingChangedEvent(notification.RoomModel, notification.LightingModel);
        return Task.WhenAll(
            hausBus.PublishAsync(RoutableCommand.FromEvent(@event), cancellationToken),
            hausBus.PublishAsync(RoutableEvent.FromEvent(@event), cancellationToken)
        );
    }
}
