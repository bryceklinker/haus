using System.Text.Json;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers
{
    public class BridgeMessageMapperTests
    {
        private const string EventsTopicName = "events";
        private const string UnknownTopicName = "unknown";
        private readonly BridgeMessageMapper _mapper;

        public BridgeMessageMapperTests()
        {
            var options = Options.Create(new HausOptions
            {
                EventsTopic = EventsTopicName,
                UnknownTopic = UnknownTopicName
            });
            _mapper = new BridgeMessageMapper(options);
        }

        [Fact]
        public void WhenInterviewSuccessfulThenReturnsEventsTopic()
        {
            var message = Zigbee2MqttMessage.FromJObject("", Zigbee2MqttMessages.InterviewSuccessfulJObject(""));
            var result = _mapper.Map(message);

            Assert.Equal(EventsTopicName, result.Topic);
        }

        [Fact]
        public void WhenUnknownMessageThenReturnsUnknownTopic()
        {
            var message = new Zigbee2MqttMessage("something", "my-payload");

            var result = _mapper.Map(message);
            var model = JsonSerializer.Deserialize<UnknownModel>(result.Payload);

            Assert.Equal(UnknownTopicName, result.Topic);
            Assert.Equal("something", model.Topic);
            Assert.Equal("my-payload", model.Payload);
        }
    }
}