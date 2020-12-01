using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus
{
    public interface IMapper
    {
        MqttApplicationMessage Map(Zigbee2MqttMessage message);
    }
}