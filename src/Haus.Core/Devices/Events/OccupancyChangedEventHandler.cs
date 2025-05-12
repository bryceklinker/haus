using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events;

internal class OccupancyChangedEventHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
    : IEventHandler<RoutableEvent<OccupancyChangedModel>>
{
    public async Task Handle(RoutableEvent<OccupancyChangedModel> notification, CancellationToken cancellationToken)
    {
        var deviceExternalId = notification.Payload.DeviceId;
        var room = await repository
            .GetRoomByDeviceExternalId(deviceExternalId, cancellationToken)
            .ConfigureAwait(false);
        if (room == null)
            return;

        room.ChangeOccupancy(notification.Payload, domainEventBus);

        await repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
