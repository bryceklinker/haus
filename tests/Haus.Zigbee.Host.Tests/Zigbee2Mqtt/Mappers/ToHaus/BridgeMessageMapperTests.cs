using Haus.Core.Models;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus
{
    public class BridgeMessageMapperTests
    {
        private const string EventsTopicName = "events";
        private const string UnknownTopicName = "unknown";
        private readonly BridgeMessageMapper _mapper;

        public BridgeMessageMapperTests()
        {
            var options = new OptionsMonitorFake<HausOptions>(new HausOptions
            {
                EventsTopic = EventsTopicName,
                UnknownTopic = UnknownTopicName
            });
            _mapper = new BridgeMessageMapper(options);
        }

        [Fact]
        public void WhenInterviewSuccessfulThenReturnsEventsTopic()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithPairingType()
                .WithMeta(meta => meta.WithFriendlyName(""))
                .WithInterviewSuccessful()
                .BuildZigbee2MqttMessage();
            
            var result = _mapper.Map(message);

            Assert.Equal(EventsTopicName, result.Topic);
        }

        [Fact]
        public void WhenUnknownMessageThenReturnsUnknownTopic()
        {
            var message = new Zigbee2MqttMessage("something", "my-payload");

            var result = _mapper.Map(message);
            var model = HausJsonSerializer.Deserialize<UnknownModel>(result.Payload);

            Assert.Equal(UnknownTopicName, result.Topic);
            Assert.Equal("something", model.Topic);
            Assert.Equal("my-payload", model.Payload);
        }
    }
}