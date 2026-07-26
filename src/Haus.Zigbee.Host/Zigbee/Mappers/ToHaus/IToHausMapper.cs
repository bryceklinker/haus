using System.Collections.Generic;
using Haus.Zigbee.Host.Zigbee.Models;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;

public interface IToHausMapper
{
    bool IsSupported(Zigbee2MqttMessage message);
    IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message);
}
