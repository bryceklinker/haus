using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Rooms.Events
{
    public class RoomLightingChanged : IHausCommandCreator<RoomLightingChanged>
    {
        public const string Type = "room_lighting_changed";
        public RoomModel Room { get; }
        public LightingModel Lighting { get; }

        public RoomLightingChanged(RoomModel room, LightingModel lighting)
        {
            Room = room;
            Lighting = lighting;
        }

        public HausCommand<RoomLightingChanged> AsHausCommand()
        {
            return new HausCommand<RoomLightingChanged>(Type, this);
        }
    }
}