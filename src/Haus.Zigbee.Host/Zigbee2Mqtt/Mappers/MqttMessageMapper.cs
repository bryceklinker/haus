using System;
using System.Linq;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;

public interface IMqttMessageMapper
{
    MqttApplicationMessage[] Map(MqttApplicationMessage original);
}

public class MqttMessageMapper(
    IOptionsMonitor<HausOptions> hausOptions,
    IOptions<ZigbeeOptions> zigbeeOptions,
    IZigbeeToHausMapper zigbeeToHausMapper,
    IHausToZigbeeMapper hausToZigbeeMapper)
    : IMqttMessageMapper
{
    private string Zigbee2MqttBaseTopic => zigbeeOptions.Value.Config.Mqtt.BaseTopic;
    private string HausCommandsTopic => hausOptions.CurrentValue.CommandsTopic;

    public MqttApplicationMessage[] Map(MqttApplicationMessage original)
    {
        if (original.Topic.StartsWith(Zigbee2MqttBaseTopic))
            return zigbeeToHausMapper.Map(original).ToArray();

        if (original.Topic == HausCommandsTopic)
            return hausToZigbeeMapper.Map(original).ToArray();

        return [];
    }
}