using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Unknown;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Mappers;
using Haus.Zigbee.Host.Tests.Support;
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
            _mapper = new PairingMessageMapper(hausOptions, new DeviceTypeResolver());
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

            var result = _mapper.Map(message);

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
                    .WithVendor("Phillips")
                )
                .BuildZigbee2MqttMessage();
            var result = _mapper.Map(message);

            var hausEvent = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredModel>>(result.Payload);
            Assert.Equal(DeviceDiscoveredModel.Type, hausEvent.Type);
            Assert.Equal("this-is-an-id", hausEvent.Payload.Id);
            Assert.Equal("my description", hausEvent.Payload.Description);
            Assert.Equal("this is a model", hausEvent.Payload.Model);
            Assert.Equal("Phillips", hausEvent.Payload.Vendor);
            Assert.Equal(DeviceType.Unknown, hausEvent.Payload.DeviceType);
        }

        [Fact]
        public void WhenInterviewStartedThenReturnsUnknownEvent()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithInterviewStarted()
                .WithPairingType()
                .WithLogTopic()
                .WithMeta(meta => meta.WithFriendlyName("friendlyName"))
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);
            
            var model = HausJsonSerializer.Deserialize<UnknownModel>(result.Payload);
            Assert.Equal(UnknownTopic, result.Topic);
            Assert.Equal(Zigbee2MqttLogTopic, model.Topic);
        }
    }
}