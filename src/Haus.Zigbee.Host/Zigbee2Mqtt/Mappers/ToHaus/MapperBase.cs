using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus
{
    public abstract class MapperBase : IMapper
    {
        private readonly IOptionsMonitor<HausOptions> _hausOptions;

        protected string EventTopicName => _hausOptions.CurrentValue.EventsTopic;
        
        protected MapperBase(IOptionsMonitor<HausOptions> hausOptions)
        {
            _hausOptions = hausOptions;
        }

        public abstract MqttApplicationMessage Map(Zigbee2MqttMessage message);
    }
}