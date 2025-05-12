using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events;

public record DeviceUpdatedEvent(DeviceModel Device) : IHausEventCreator<DeviceUpdatedEvent>
{
    public const string Type = "device_updated";

    public HausEvent<DeviceUpdatedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
