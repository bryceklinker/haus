namespace Haus.Core.Models.Sensors.Battery
{
    public class BatteryChangedModel
    {
        public string DeviceId { get; set; }
        public long BatteryLevel { get; set; }
    }
}