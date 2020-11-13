using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers
{
    public interface IZigbeeToHausModelMapper
    {
        MqttApplicationMessage Map(MqttApplicationMessage message);
    }

    public class ZigbeeToHausModelMapper : IZigbeeToHausModelMapper
    {
        private readonly IMapperFactory _mapperFactory;

        public ZigbeeToHausModelMapper(IOptions<HausOptions> hausOptions, IOptions<ZigbeeOptions> zigbeeOptions)
            : this(new MapperFactory(hausOptions, zigbeeOptions))
        {
        }

        public ZigbeeToHausModelMapper(IMapperFactory mapperFactory)
        {
            _mapperFactory = mapperFactory;
        }

        public MqttApplicationMessage Map(MqttApplicationMessage message)
        {
            var payload = new Zigbee2MqttMessage(message.Topic, message.Payload);
            var mapper = _mapperFactory.GetMapper(message.Topic);
            return mapper.Map(payload);
        }
    }
}