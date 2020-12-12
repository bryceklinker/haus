using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus
{
    public class InterviewSuccessfulMapperTests
    {
        private const string HausEventTopic = "idk";
        private readonly InterviewSuccessfulMapper _mapper;

        public InterviewSuccessfulMapperTests()
        {
            var hausOptions = OptionsFactory.CreateHausOptions(eventsTopic: HausEventTopic);
            var zigbeeOptions = OptionsFactory.CreateZigbeeOptions();
            _mapper = new InterviewSuccessfulMapper(hausOptions, zigbeeOptions);
        }

        [Fact]
        public void WhenMessageHasLogTopicAndTypePairingAndMessageInterviewSuccessfulThenIsSupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .WithPairingType()
                .WithInterviewSuccessful()
                .BuildZigbee2MqttMessage();
            
            Assert.True(_mapper.IsSupported(message));
        }

        [Fact]
        public void WhenMessageTopicIsNotLogTopicThenUnsupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithTopicPath("idk")
                .WithPairingType()
                .WithInterviewSuccessful()
                .BuildZigbee2MqttMessage();
            
            Assert.False(_mapper.IsSupported(message));
        }

        [Fact]
        public void WhenMessageTypeIsNotPairingThenUnsupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .WithType("nope")
                .WithInterviewSuccessful()
                .BuildZigbee2MqttMessage();
            
            Assert.False(_mapper.IsSupported(message));
        }

        [Fact]
        public void WhenMessageIsNotInterviewSuccessfulThenUnsupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .WithPairingType()
                .WithMessage("not good")
                .BuildZigbee2MqttMessage();
            
            Assert.False(_mapper.IsSupported(message));
        }
        
        [Fact]
        public void WhenInterviewSuccessfulReceivedThenReturnsHausEvent()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .WithInterviewSuccessful()
                .WithPairingType()
                .WithMeta(meta => meta.WithFriendlyName("idk"))
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message).Single();

            Assert.Equal(HausEventTopic, result.Topic);
        }

        [Fact]
        public void WhenInterviewSuccessfulMappedThenReturnsDeviceDiscoveredMessage()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithInterviewSuccessful()
                .WithPairingType()
                .WithLogTopic()
                .WithMeta(meta => meta.WithFriendlyName("this-is-an-id")
                    .WithDescription("my description")
                    .WithModel("this is a model")
                    .WithSupported(true)
                    .WithVendor("Philips")
                )
                .BuildZigbee2MqttMessage();
            var result = _mapper.Map(message).Single();

            var hausEvent = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredModel>>(result.Payload);
            Assert.Equal(DeviceDiscoveredModel.Type, hausEvent.Type);
            Assert.Equal("this-is-an-id", hausEvent.Payload.Id);
            Assert.Equal("my description", hausEvent.Payload.Description);
            Assert.Equal("this is a model", hausEvent.Payload.Model);
            Assert.Equal("Philips", hausEvent.Payload.Vendor);
            Assert.Equal(DeviceType.Unknown, hausEvent.Payload.DeviceType);
        }

        [Fact]
        public void WhenInterviewSuccessfulThenReturnsDeviceDiscoveredWithLightDeviceType()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithInterviewSuccessful()
                .WithPairingType()
                .WithLogTopic()
                .WithMeta(meta => meta.WithFriendlyName("this-is-an-id")
                    .WithDescription("my description")
                    .WithModel("929002335001")
                    .WithSupported(true)
                    .WithVendor("Philips")
                )
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message).Single();
            
            var hausEvent = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredModel>>(result.Payload);
            Assert.Equal(DeviceType.Light, hausEvent.Payload.DeviceType);
        }
    }
}