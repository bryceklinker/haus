namespace Haus.Core.Models.Sensors.Motion
{
    public class OccupancyChangedModel
    {
        public const string Type = "occupancy_changed";
        public string DeviceId { get; set; }
        public bool Occupancy { get; set; }
        public long Timeout { get; set; }
        public string Sensitivity { get; set; }
    }
}