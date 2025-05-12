namespace Haus.Core.Models.Devices.Sensors.Battery;

public record BatteryChangedModel(string DeviceId, long BatteryLevel)
{
    public const string Type = "battery_changed";
}
