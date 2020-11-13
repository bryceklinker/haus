using Haus.Core.Models.Sensors.Battery;
using Haus.Core.Models.Sensors.Light;
using Haus.Core.Models.Sensors.Motion;
using Haus.Core.Models.Sensors.Temperature;

namespace Haus.Core.Models.Sensors
{
    public class MultiSensorChanged
    {
        public string DeviceId { get; set; }
        
        public bool HasTemperature => TemperatureChanged != null;
        public bool HasOccupancy => OccupancyChanged != null;
        public bool HasIlluminance => IlluminanceChanged != null;
        public bool HasBattery => BatteryChanged != null;
        
        public TemperatureChangedModel TemperatureChanged { get; set; }
        public OccupancyChangedModel OccupancyChanged { get; set; }
        public IlluminanceChangedModel IlluminanceChanged { get; set; }
        public BatteryChangedModel BatteryChanged { get; set; }
    }
}