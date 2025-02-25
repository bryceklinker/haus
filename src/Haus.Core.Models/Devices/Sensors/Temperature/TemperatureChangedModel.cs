namespace Haus.Core.Models.Devices.Sensors.Temperature;

public record TemperatureChangedModel(string DeviceId, double Temperature)
{
    public const string Type = "temperature_sensor_changed";
}
