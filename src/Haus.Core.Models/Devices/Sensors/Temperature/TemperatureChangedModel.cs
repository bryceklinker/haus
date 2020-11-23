namespace Haus.Core.Models.Devices.Sensors.Temperature
{
    public class TemperatureChangedModel
    {
        public const string Type = "temperature_sensor_changed";
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
    }
}