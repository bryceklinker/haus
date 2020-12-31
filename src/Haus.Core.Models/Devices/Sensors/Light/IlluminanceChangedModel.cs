namespace Haus.Core.Models.Devices.Sensors.Light
{
    public record IlluminanceChangedModel(string DeviceId, long Illuminance, long? Lux)
    {
        public const string Type = "illuminance_changed";
    }
}