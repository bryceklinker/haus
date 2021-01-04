using System.Linq;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Sensors
{
    public record MultiSensorChanged(
        string DeviceId,
        OccupancyChangedModel OccupancyChanged = null,
        TemperatureChangedModel TemperatureChanged = null,
        IlluminanceChangedModel IlluminanceChanged = null,
        BatteryChangedModel BatteryChanged = null) : IHausEventCreator<MultiSensorChanged>
    {
        public const string Type = "multi_sensor_changed";
        
        public bool HasTemperature => TemperatureChanged != null;
        public bool HasOccupancy => OccupancyChanged != null;
        public bool HasIlluminance => IlluminanceChanged != null;
        public bool HasBattery => BatteryChanged != null;
        public bool[] Changes => new[] {HasTemperature, HasOccupancy, HasIlluminance, HasBattery};
        public bool HasMultipleChanges => Changes.Count(c => c) > 1;
        
        public object GetSingleChange()
        {
            if (HasTemperature)
                return TemperatureChanged;

            if (HasOccupancy)
                return OccupancyChanged;

            if (HasIlluminance)
                return IlluminanceChanged;

            return BatteryChanged;
        }

        public HausEvent<MultiSensorChanged> AsHausEvent() =>new(Type, this);
    }
}