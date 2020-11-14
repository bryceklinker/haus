namespace Haus.Core.Models.Sensors.Battery
{
    public class BatteryChangedModel
    {
        public const string Type = "battery_changed";
        public string DeviceId { get; set; }
        public long BatteryLevel { get; set; }
    }
}