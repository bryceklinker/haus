using Haus.Core.Models;
using Haus.Core.Models.Sensors.Motion;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices
{
    public class OccupancyChangedMapper
    {
        public OccupancyChangedModel Map(Zigbee2MqttMessage message)
        {
            if (message.Occupancy.IsNull())
                return null;
            
            return new OccupancyChangedModel
            {
                DeviceId = message.GetFriendlyNameFromTopic(),
                Timeout = message.OccupancyTimeout.GetValueOrDefault(),
                Occupancy = message.Occupancy.GetValueOrDefault(),
                Sensitivity = message.MotionSensitivity
            };
        }
    }
}