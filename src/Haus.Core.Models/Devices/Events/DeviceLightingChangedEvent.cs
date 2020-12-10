using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events
{
    public class DeviceLightingChangedEvent : IHausCommandCreator<DeviceLightingChangedEvent>
    {
        public const string Type = "device_lighting_changed";
        public DeviceModel Device { get; }
        public LightingModel Lighting { get; }

        public DeviceLightingChangedEvent(DeviceModel device, LightingModel lighting)
        {
            Device = device;
            Lighting = lighting;
        }

        public HausCommand<DeviceLightingChangedEvent> AsHausCommand()
        {
            return new HausCommand<DeviceLightingChangedEvent>(Type, this);
        }
    }
}