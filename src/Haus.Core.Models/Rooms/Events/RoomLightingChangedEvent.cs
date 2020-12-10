using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Rooms.Events
{
    public class RoomLightingChangedEvent : IHausCommandCreator<RoomLightingChangedEvent>
    {
        public const string Type = "room_lighting_changed";
        public RoomModel Room { get; }
        public LightingModel Lighting { get; }

        public RoomLightingChangedEvent(RoomModel room, LightingModel lighting)
        {
            Room = room;
            Lighting = lighting;
        }

        public HausCommand<RoomLightingChangedEvent> AsHausCommand()
        {
            return new HausCommand<RoomLightingChangedEvent>(Type, this);
        }
    }
}