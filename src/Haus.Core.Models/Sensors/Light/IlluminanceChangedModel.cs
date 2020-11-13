namespace Haus.Core.Models.Sensors.Light
{
    public class IlluminanceChangedModel
    {
        public string DeviceId { get; set; }
        public long Illuminance { get; set; }
        public long Lux { get; set; }
    }
}