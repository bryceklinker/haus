using System;
using System.Linq;
using Haus.Core.Models.Sensors.Battery;
using Haus.Core.Models.Sensors.Light;
using Haus.Core.Models.Sensors.Motion;
using Haus.Core.Models.Sensors.Temperature;

namespace Haus.Core.Models.Sensors
{
    public class MultiSensorChanged
    {
        public const string Type = "multi_sensor_changed";
        
        public string DeviceId { get; set; }
        public bool HasTemperature => TemperatureChanged != null;
        public bool HasOccupancy => OccupancyChanged != null;
        public bool HasIlluminance => IlluminanceChanged != null;
        public bool HasBattery => BatteryChanged != null;
        public bool[] Changes => new[] {HasTemperature, HasOccupancy, HasIlluminance, HasBattery};
        public bool HasMultipleChanges => Changes.Count(c => c) > 1;
        
        public TemperatureChangedModel TemperatureChanged { get; set; }
        public OccupancyChangedModel OccupancyChanged { get; set; }
        public IlluminanceChangedModel IlluminanceChanged { get; set; }
        public BatteryChangedModel BatteryChanged { get; set; }

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
    }
}