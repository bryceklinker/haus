using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Rooms.Events;

public record RoomUpdatedEvent(RoomModel Room) : IHausEventCreator<RoomUpdatedEvent>
{
    public const string Type = "room_updated";

    public HausEvent<RoomUpdatedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
