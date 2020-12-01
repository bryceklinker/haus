using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus
{
    public interface IZigbeeToHausMapper
    {
        MqttApplicationMessage Map(MqttApplicationMessage message);
    }

    public class ZigbeeToHausMapper : IZigbeeToHausMapper
    {
        private readonly IZigbee2MqttMessageFactory _zigbee2MqttMessageFactory;
        private readonly IMapperFactory _mapperFactory;

        public ZigbeeToHausMapper(IOptionsMonitor<HausOptions> hausOptions,
            IOptions<ZigbeeOptions> zigbeeOptions,
            IZigbee2MqttMessageFactory zigbee2MqttMessageFactory)
        {
            _zigbee2MqttMessageFactory = zigbee2MqttMessageFactory;
            _mapperFactory = new MapperFactory(hausOptions, zigbeeOptions);
        }

        public MqttApplicationMessage Map(MqttApplicationMessage message)
        {
            var payload = _zigbee2MqttMessageFactory.Create(message);
            var mapper = _mapperFactory.GetMapper(message.Topic);
            return mapper.Map(payload);
        }
    }
}