using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;

public interface IZigbeeToHausMapper
{
    IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message);
}

public class ZigbeeToHausMapper(
    IZigbee2MqttMessageFactory zigbee2MqttMessageFactory,
    IEnumerable<IToHausMapper> mappers,
    IUnknownMessageMapper unknownMessageMapper)
    : IZigbeeToHausMapper
{
    public IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message)
    {
        var zigbee2MqttMessage = zigbee2MqttMessageFactory.Create(message);

        var messages = mappers.Where(m => m.IsSupported(zigbee2MqttMessage))
            .SelectMany(m => m.Map(zigbee2MqttMessage))
            .ToArray();

        return messages.IsEmpty()
            ? unknownMessageMapper.Map(zigbee2MqttMessage)
            : messages;
    }
}