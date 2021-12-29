using System.Collections.Generic;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;

public abstract class ToHausMapperBase : IToHausMapper
{
    private readonly IOptionsMonitor<HausOptions> _hausOptions;

    protected string EventTopicName => _hausOptions.CurrentValue.EventsTopic;

    protected ToHausMapperBase(IOptionsMonitor<HausOptions> hausOptions)
    {
        _hausOptions = hausOptions;
    }

    public abstract bool IsSupported(Zigbee2MqttMessage message);

    public abstract IEnumerable<MqttApplicationMessage> Map(Zigbee2MqttMessage message);
}