using Haus.Core.Models;
using Haus.Core.Models.Sensors.Battery;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices
{
    public class BatteryChangedMapper
    {
        public BatteryChangedModel Map(Zigbee2MqttMessage message)
        {
            if (message.Battery.IsNull())
                return null;
            
            return new BatteryChangedModel
            {
                DeviceId = message.GetFriendlyNameFromTopic(),
                BatteryLevel = message.Battery.GetValueOrDefault()
            };
        }
    }
}