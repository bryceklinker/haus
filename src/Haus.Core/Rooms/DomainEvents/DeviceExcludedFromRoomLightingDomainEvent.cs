using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.DomainEvents;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Rooms.DomainEvents;

public record DeviceExcludedFromRoomLightingDomainEvent(DeviceEntity Device, RoomEntity Room) : IDomainEvent;

internal class DeviceExcludedFromRoomLightingDomainEventHandler(
    ILogger<DeviceExcludedFromRoomLightingDomainEventHandler> logger
) : IDomainEventHandler<DeviceExcludedFromRoomLightingDomainEvent>
{
    public Task Handle(DeviceExcludedFromRoomLightingDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogWarning(
            "Excluded device {@Id} from room {@RoomId} lighting change because it is not classified as a light (DeviceType=Unknown)",
            notification.Device.Id,
            notification.Room.Id
        );
        return Task.CompletedTask;
    }
}
