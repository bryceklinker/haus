using System.Collections.Generic;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;

public abstract class ToHausMapperBase(IOptionsMonitor<HausOptions> hausOptions) : IToHausMapper
{
    protected string EventTopicName => hausOptions.CurrentValue.EventsTopic;

    public abstract bool IsSupported(Zigbee2MqttMessage message);

    public abstract IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message);
}