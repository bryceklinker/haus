namespace Haus.Core.Models.Sensors.Temperature
{
    public class TemperatureChangedModel
    {
        public const string Type = "temperature_sensor_changed";
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
    }
}