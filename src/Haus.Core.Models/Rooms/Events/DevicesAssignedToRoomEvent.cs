using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Rooms.Events;

public record DevicesAssignedToRoomEvent
    (long RoomId, params long[] DeviceIds) : IHausEventCreator<DevicesAssignedToRoomEvent>
{
    public const string Type = "devices_assigned_to_room";

    public HausEvent<DevicesAssignedToRoomEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}