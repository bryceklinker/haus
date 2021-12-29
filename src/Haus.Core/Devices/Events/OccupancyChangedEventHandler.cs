using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events;

internal class OccupancyChangedEventHandler : IEventHandler<RoutableEvent<OccupancyChangedModel>>
{
    private readonly IRoomCommandRepository _repository;
    private readonly IDomainEventBus _domainEventBus;

    public OccupancyChangedEventHandler(IDomainEventBus domainEventBus, IRoomCommandRepository repository)
    {
        _domainEventBus = domainEventBus;
        _repository = repository;
    }

    public async Task Handle(RoutableEvent<OccupancyChangedModel> notification, CancellationToken cancellationToken)
    {
        var deviceExternalId = notification.Payload.DeviceId;
        var room = await _repository.GetRoomByDeviceExternalId(deviceExternalId, cancellationToken)
            .ConfigureAwait(false);
        if (room == null)
            return;

        room.ChangeOccupancy(notification.Payload, _domainEventBus);

        await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}