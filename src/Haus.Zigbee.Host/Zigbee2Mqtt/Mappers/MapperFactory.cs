using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Services;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers
{
    public interface IMapperFactory
    {
        IMapper GetMapper(string topic);
    }

    public class MapperFactory : IMapperFactory
    {
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
        private readonly BridgeMessageMapper _bridgeMessageMapper;
        private readonly UnknownMessageMapper _unknownMessageMapper;

        private string ZigbeeTopicName => _zigbeeOptions.Value.Config.Mqtt.BaseTopic;
        
        public MapperFactory(IOptions<HausOptions> hausOptions, IOptions<ZigbeeOptions> zigbeeOptions)
            : this(
                new BridgeMessageMapper(hausOptions),
                new UnknownMessageMapper(hausOptions), 
                zigbeeOptions)
        {
        }
        
        public MapperFactory(
            BridgeMessageMapper bridgeMessageMapper,
            UnknownMessageMapper unknownMessageMapper,
            IOptions<ZigbeeOptions> zigbeeOptions)
        {
            _bridgeMessageMapper = bridgeMessageMapper;
            _unknownMessageMapper = unknownMessageMapper;
            _zigbeeOptions = zigbeeOptions;
        }

        public IMapper GetMapper(string topic)
        {
            if (topic.StartsWith($"{ZigbeeTopicName}/bridge"))
                return _bridgeMessageMapper;

            return _unknownMessageMapper;
        }
    }
}