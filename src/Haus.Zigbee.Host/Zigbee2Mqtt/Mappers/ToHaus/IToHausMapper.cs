using System.Collections.Generic;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;

public interface IToHausMapper
{
    bool IsSupported(Zigbee2MqttMessage message);
    IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message);
}
