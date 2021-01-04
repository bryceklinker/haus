using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Models.Devices.Events
{
    public record DeviceLightingChangedEvent(DeviceModel Device, LightingModel Lighting) : 
        IHausCommandCreator<DeviceLightingChangedEvent>,
        IHausEventCreator<DeviceLightingChangedEvent>
    {
        public const string Type = "device_lighting_changed";

        public HausCommand<DeviceLightingChangedEvent> AsHausCommand() => new(Type, this);
        public HausEvent<DeviceLightingChangedEvent> AsHausEvent() => new(Type, this);
    }
}