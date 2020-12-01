using Haus.Core.Models;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Devices
{
    public class TemperatureChangedMapper
    {
        public TemperatureChangedModel Map(Zigbee2MqttMessage message)
        {
            if (message.Temperature.IsNull())
                return null;
            
            return new TemperatureChangedModel
            {
                DeviceId = message.GetFriendlyNameFromTopic(),
                Temperature = message.Temperature.GetValueOrDefault()
            };
        }
    }
}