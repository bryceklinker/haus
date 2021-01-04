using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Rooms.Events
{
    public record RoomCreatedEvent(RoomModel Room) : IHausEventCreator<RoomCreatedEvent>
    {
        public const string Type = "room_created";

        public HausEvent<RoomCreatedEvent> AsHausEvent() => new(Type, this);
    }
}