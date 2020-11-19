using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices;
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
        private readonly IOptions<HausOptions> _hausOptions;
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;

        private string ZigbeeTopicName => _zigbeeOptions.Value.Config.Mqtt.BaseTopic;
        
        public MapperFactory(IOptions<HausOptions> hausOptions, IOptions<ZigbeeOptions> zigbeeOptions)
        {
            _hausOptions = hausOptions;
            _zigbeeOptions = zigbeeOptions;
        }
        
        public IMapper GetMapper(string topic)
        {
            if (topic.Split('/').Length == 2)
                return new DeviceMessageMapper(_hausOptions);
            
            if (topic.StartsWith($"{ZigbeeTopicName}/bridge"))
                return new BridgeMessageMapper(_hausOptions);

            return new UnknownMessageMapper(_hausOptions);
        }
    }
}