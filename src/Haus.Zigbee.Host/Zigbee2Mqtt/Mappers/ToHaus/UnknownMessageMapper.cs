using System.Collections.Generic;
using Haus.Core.Models;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;

public interface IUnknownMessageMapper : IToHausMapper { }

public class UnknownMessageMapper(IOptionsMonitor<HausOptions> options) : IUnknownMessageMapper
{
    private string UnknownTopicName => options.CurrentValue.UnknownTopic;

    public bool IsSupported(Zigbee2MqttMessage message)
    {
        return false;
    }

    public IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message)
    {
        var model = new UnknownModel(message.Topic, message.Json);
        yield return new MqttApplicationMessage
        {
            Topic = UnknownTopicName,
            PayloadSegment = HausJsonSerializer.SerializeToBytes(model),
        };
    }
}
