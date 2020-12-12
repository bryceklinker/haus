using System.Collections.Generic;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee
{
    public interface IToZigbeeMapper
    {
        bool IsSupported(string type);
        IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message);
    }
}