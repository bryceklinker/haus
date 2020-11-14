using Haus.Core.Models;
using Haus.Core.Models.Sensors.Light;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices
{
    public class IlluminanceChangedMapper
    {
        public IlluminanceChangedModel Map(Zigbee2MqttMessage message)
        {
            if (message.Illuminance.IsNull())
                return null;
            
            return new IlluminanceChangedModel
            {
                DeviceId = message.GetFriendlyNameFromTopic(),
                Illuminance = message.Illuminance.GetValueOrDefault(),
                Lux = message.IlluminanceLux.GetValueOrDefault()
            };   
        }
    }
}