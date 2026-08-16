using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events;

public record DeviceDeletedEvent(DeviceModel Device) : IHausEventCreator<DeviceDeletedEvent>
{
    public const string Type = "device_deleted";

    public HausEvent<DeviceDeletedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
