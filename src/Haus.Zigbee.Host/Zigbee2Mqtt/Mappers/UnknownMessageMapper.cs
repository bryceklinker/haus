using Haus.Core.Models;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mappers
{
    public class UnknownMessageMapper : IMapper
    {
        private readonly IOptionsMonitor<HausOptions> _options;

        private string UnknownTopicName => _options.CurrentValue.UnknownTopic;
        
        public UnknownMessageMapper(IOptionsMonitor<HausOptions> options)
        {
            _options = options;
        }

        public MqttApplicationMessage Map(Zigbee2MqttMessage message)
        {
            var model = new UnknownModel
            {
                Topic = message.Topic,
                Payload = message.Json
            };
            return new MqttApplicationMessage
            {
                Topic = UnknownTopicName,
                Payload = HausJsonSerializer.SerializeToBytes(model)
            };
        }
    }
}