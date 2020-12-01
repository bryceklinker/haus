using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Devices;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus
{
    public interface IMapperFactory
    {
        IMapper GetMapper(string topic);
    }

    public class MapperFactory : IMapperFactory
    {
        private readonly IOptionsMonitor<HausOptions> _hausOptions;
        private readonly IOptions<ZigbeeOptions> _zigbeeOptions;

        private string ZigbeeTopicName => _zigbeeOptions.Value.Config.Mqtt.BaseTopic;
        
        public MapperFactory(IOptionsMonitor<HausOptions> hausOptions, IOptions<ZigbeeOptions> zigbeeOptions)
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