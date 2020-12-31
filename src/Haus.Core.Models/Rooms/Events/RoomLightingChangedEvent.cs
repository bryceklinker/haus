using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Rooms.Events
{
    public record RoomLightingChangedEvent(RoomModel Room, LightingModel Lighting) : IHausCommandCreator<RoomLightingChangedEvent>
    {
        public const string Type = "room_lighting_changed";

        public HausCommand<RoomLightingChangedEvent> AsHausCommand()
        {
            return new(Type, this);
        }
    }
}