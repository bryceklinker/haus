using System.Linq;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Sensors
{
    public class MultiSensorChanged : IHausEventCreator<MultiSensorChanged>
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

        public HausEvent<MultiSensorChanged> AsHausEvent()
        {
            return new HausEvent<MultiSensorChanged>(Type, this);
        }
    }
}