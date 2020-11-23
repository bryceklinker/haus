namespace Haus.Core.Models.Devices.Sensors.Light
{
    public class IlluminanceChangedModel
    {
        public const string Type = "illuminance_changed";
        public string DeviceId { get; set; }
        public long Illuminance { get; set; }
        public long? Lux { get; set; }
    }
}