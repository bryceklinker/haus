using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Sensors.Motion;

public record OccupancyChangedModel(
    string DeviceId,
    bool Occupancy = false,
    long Timeout = 0,
    string? Sensitivity = null
) : IHausEventCreator<OccupancyChangedModel>
{
    public const string Type = "occupancy_changed";

    public HausEvent<OccupancyChangedModel> AsHausEvent()
    {
        return new HausEvent<OccupancyChangedModel>(Type, this);
    }
}
