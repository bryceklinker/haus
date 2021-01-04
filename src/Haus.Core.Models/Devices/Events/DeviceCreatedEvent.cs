using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events
{
    public record DeviceCreatedEvent(DeviceModel device) : IHausEventCreator<DeviceCreatedEvent>
    {
        public const string Type = "device_created";
        public HausEvent<DeviceCreatedEvent> AsHausEvent() => new(Type, this);
    }
}