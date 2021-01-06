using System.Linq;
using FluentAssertions;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
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

            _mapper.IsSupported(message).Should().BeTrue();
        }

        [Fact]
        public void WhenMessageTopicIsNotLogTopicThenUnsupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithTopicPath("idk")
                .WithPairingType()
                .WithInterviewSuccessful()
                .BuildZigbee2MqttMessage();

            _mapper.IsSupported(message).Should().BeFalse();
        }

        [Fact]
        public void WhenMessageTypeIsNotPairingThenUnsupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .WithType("nope")
                .WithInterviewSuccessful()
                .BuildZigbee2MqttMessage();

            _mapper.IsSupported(message).Should().BeFalse();
        }

        [Fact]
        public void WhenMessageIsNotInterviewSuccessfulThenUnsupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .WithPairingType()
                .WithMessage("not good")
                .BuildZigbee2MqttMessage();

            _mapper.IsSupported(message).Should().BeFalse();
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

            result.Topic.Should().Be(HausEventTopic);
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

            var hausEvent = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredEvent>>(result.Payload);
            hausEvent.Type.Should().Be(DeviceDiscoveredEvent.Type);
            hausEvent.Payload.Id.Should().Be("this-is-an-id");
            hausEvent.Payload.DeviceType.Should().Be(DeviceType.Unknown);
            hausEvent.Payload.Metadata.Should()
                .ContainEquivalentOf(new MetadataModel("description", "my description"))
                .And.ContainEquivalentOf(new MetadataModel("model", "this is a model"))
                .And.ContainEquivalentOf(new MetadataModel("vendor", "Philips"));
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
            
            var hausEvent = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredEvent>>(result.Payload);
            hausEvent.Payload.DeviceType.Should().Be(DeviceType.Light);
        }
    }
}