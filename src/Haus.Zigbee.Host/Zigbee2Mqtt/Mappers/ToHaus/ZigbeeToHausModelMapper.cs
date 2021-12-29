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

public class ZigbeeToHausMapper : IZigbeeToHausMapper
{
    private readonly IZigbee2MqttMessageFactory _zigbee2MqttMessageFactory;
    private readonly IEnumerable<IToHausMapper> _mappers;
    private readonly IUnknownMessageMapper _unknownMessageMapper;

    public ZigbeeToHausMapper(
        IZigbee2MqttMessageFactory zigbee2MqttMessageFactory,
        IEnumerable<IToHausMapper> mappers,
        IUnknownMessageMapper unknownMessageMapper)
    {
        _zigbee2MqttMessageFactory = zigbee2MqttMessageFactory;
        _mappers = mappers;
        _unknownMessageMapper = unknownMessageMapper;
    }

    public IEnumerable<MqttApplicationMessage> Map(MqttApplicationMessage message)
    {
        var zigbee2MqttMessage = _zigbee2MqttMessageFactory.Create(message);

        var messages = _mappers.Where(m => m.IsSupported(zigbee2MqttMessage))
            .SelectMany(m => m.Map(zigbee2MqttMessage))
            .ToArray();

        return messages.IsEmpty()
            ? _unknownMessageMapper.Map(zigbee2MqttMessage)
            : messages;
    }
}