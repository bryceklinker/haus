using Haus.Core.Models;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Zigbee.Host.Zigbee.Models;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;

public class OccupancyChangedMapper
{
    public OccupancyChangedModel? Map(Zigbee2MqttMessage message)
    {
        if (message.Occupancy.IsNull())
            return null;

        return new OccupancyChangedModel(
            message.GetFriendlyNameFromTopic(),
            message.Occupancy.GetValueOrDefault(),
            message.OccupancyTimeout.GetValueOrDefault(),
            message.MotionSensitivity
        );
    }
}
