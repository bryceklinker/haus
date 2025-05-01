using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events;

public record DeviceCreatedEvent(DeviceModel Device) : IHausEventCreator<DeviceCreatedEvent>
{
    public const string Type = "device_created";

    public HausEvent<DeviceCreatedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
