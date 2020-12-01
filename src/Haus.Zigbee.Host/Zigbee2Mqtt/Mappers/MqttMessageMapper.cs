using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers
{
    public interface IMqttMessageMapper
    {
        MqttApplicationMessage Map(MqttApplicationMessage original);
    }

    public class MqttMessageMapper : IMqttMessageMapper
    {
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
        private readonly IZigbeeToHausMapper _zigbeeToHausMapper;
        private readonly IHausToZigbeeMapper _hausToZigbeeMapper;
        private readonly IOptionsMonitor<HausOptions> _hausOptions;

        private string Zigbee2MqttBaseTopic => _zigbeeOptions.Value.Config.Mqtt.BaseTopic;
        private string HausCommandsTopic => _hausOptions.CurrentValue.CommandsTopic;
        
        public MqttMessageMapper(IOptionsMonitor<HausOptions> hausOptions,
            IOptions<ZigbeeOptions> zigbeeOptions,
            IZigbeeToHausMapper zigbeeToHausMapper, 
            IHausToZigbeeMapper hausToZigbeeMapper)
        {
            _hausOptions = hausOptions;
            _zigbeeOptions = zigbeeOptions;
            _zigbeeToHausMapper = zigbeeToHausMapper;
            _hausToZigbeeMapper = hausToZigbeeMapper;
        }

        public MqttApplicationMessage Map(MqttApplicationMessage original)
        {
            if (original.Topic.StartsWith(Zigbee2MqttBaseTopic))
                return _zigbeeToHausMapper.Map(original);

            if (original.Topic == HausCommandsTopic)
                return _hausToZigbeeMapper.Map(original);

            return null;
        }
    }
}