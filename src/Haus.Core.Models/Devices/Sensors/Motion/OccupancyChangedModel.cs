using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Sensors.Motion
{
    public class OccupancyChangedModel : IHausEventCreator<OccupancyChangedModel>
    {
        public const string Type = "occupancy_changed";
        public string DeviceId { get; set; }
        public bool Occupancy { get; set; }
        public long Timeout { get; set; }
        public string Sensitivity { get; set; }
        public HausEvent<OccupancyChangedModel> AsHausEvent()
        {
            return new HausEvent<OccupancyChangedModel>(Type, this);
        }
    }
}