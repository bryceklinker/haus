using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Pairing;
using Haus.Zigbee.Host.Zigbee2Mqtt.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers.Pairing
{
    public class PairingMessageMapperTests
    {
        private const string Zigbee2MqttTopic = "zigbee2mqtt";
        private static readonly string Zigbee2MqttLogTopic = $"{Zigbee2MqttTopic}/bridge/log";
        private const string HausEventTopic = "haus/events";
        private const string UnknownTopic = "haus/unknown";
        private readonly PairingMessageMapper _mapper;

        public PairingMessageMapperTests()
        {
            var hausOptions = Options.Create(new HausOptions
            {
                EventsTopic = HausEventTopic,
                UnknownTopic = UnknownTopic
            });
            _mapper = new PairingMessageMapper(hausOptions);
        }

        [Fact]
        public void WhenInterviewSuccessfulReceivedThenReturnsHausEvent()
        {
            var message = Zigbee2MqttMessage.FromJObject(
                Zigbee2MqttLogTopic,
                Zigbee2MqttMessages.InterviewSuccessfulJObject("idk"));

            var result = _mapper.Map(message);

            Assert.Equal(HausEventTopic, result.Topic);
        }

        [Fact]
        public void WhenInterviewSuccessfulMappedThenReturnsDeviceDiscoveredMessage()
        {
            var message = Zigbee2MqttMessage.FromJObject(
                Zigbee2MqttLogTopic,
                Zigbee2MqttMessages.InterviewSuccessfulJObject(
                    "this-is-an-id",
                    "my description",
                    "this is a model",
                    true,
                    "Phillips"));
            var result = _mapper.Map(message);

            var hausEvent = JsonSerializer.Deserialize<HausEvent<DeviceDiscoveredModel>>(result.Payload);
            Assert.Equal(DeviceDiscoveredModel.Type, hausEvent.Type);
            Assert.Equal("this-is-an-id", hausEvent.Payload.Id);
            Assert.Equal("my description", hausEvent.Payload.Description);
            Assert.Equal("this is a model", hausEvent.Payload.Model);
            Assert.Equal("Phillips", hausEvent.Payload.Vendor);
        }

        [Fact]
        public void WhenInterviewStartedThenReturnsUnknownEvent()
        {
            var message = Zigbee2MqttMessage.FromJObject(
                Zigbee2MqttLogTopic,
                Zigbee2MqttMessages.InterviewStartedJObject());

            var result = _mapper.Map(message);
            
            var model = JsonSerializer.Deserialize<UnknownModel>(result.Payload);
            Assert.Equal(UnknownTopic, result.Topic);
            Assert.Equal(Zigbee2MqttLogTopic, model.Topic);
        }
    }
}