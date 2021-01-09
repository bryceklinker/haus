using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Models.Devices.Events
{
    public record DeviceLightingConstraintsChangedEvent(DeviceModel Device, LightingConstraintsModel Constraints)
        : IHausEventCreator<DeviceLightingConstraintsChangedEvent>
    {
        public const string Type = "device_lighting_constraints_changed";

        public HausEvent<DeviceLightingConstraintsChangedEvent> AsHausEvent() => new(Type, this);
    }
}