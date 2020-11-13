using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Discovery;
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
        private readonly BridgeMessageMapper _mapper;

        public BridgeMessageMapperTests()
        {
            var options = Options.Create(new HausOptions
            {
                EventsTopic = EventsTopicName
            });
            _mapper = new BridgeMessageMapper(options);
        }

        [Fact]
        public void WhenInterviewSuccessfulThenReturnsEventsTopic()
        {
            var message = new Zigbee2MqttMessage("", Zigbee2MqttMessages.InterviewSuccessfulJObject(""));
            var result = _mapper.Map(message);

            Assert.Equal(EventsTopicName, result.Topic);
        }
        
        [Fact]
        public void WhenInterviewSuccessfulMappedThenReturnsDeviceDiscoveredMessage()
        {
            var message = new Zigbee2MqttMessage("", Zigbee2MqttMessages.InterviewSuccessfulJObject(
                "this-is-an-id",
                "my description",
                "this is a model",
                true,
                "Phillips"));
            var result = _mapper.Map(message);

            var hausEvent = JsonSerializer.Deserialize<HausEvent>(result.Payload);
            var hausEventPayload = hausEvent.GetPayload<DeviceDiscoveredModel>();
            Assert.Equal(DeviceDiscoveredModel.Type, hausEvent.Type);
            Assert.Equal("this-is-an-id", hausEventPayload.Id);
            Assert.Equal("my description", hausEventPayload.Description);
            Assert.Equal("this is a model", hausEventPayload.Model);
            Assert.Equal("Phillips", hausEventPayload.Vendor);
        }
    }
}