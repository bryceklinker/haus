using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Factories;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers
{
    public class ZigbeeToHausModelMapperTests
    {
        private const string Zigbee2MqttTopic = "zigbee2mqtt";
        private const string HausEventTopic = "haus/events";
        private const string UnknownEventTopic = "haus/unknown";
        private readonly ZigbeeToHausModelMapper _mapper;

        public ZigbeeToHausModelMapperTests()
        {
            var zigbeeOptions = Options.Create(new ZigbeeOptions
            {
                Config = new Zigbee2MqttConfiguration
                {
                    Mqtt = new MqttConfiguration
                    {
                        BaseTopic = Zigbee2MqttTopic
                    }
                }
            });
            var hausOptions = Options.Create(new HausOptions
            {
                EventsTopic = HausEventTopic,
                UnknownTopic = UnknownEventTopic
            });
            
            _mapper = new ZigbeeToHausModelMapper(hausOptions, zigbeeOptions, new Zigbee2MqttMessageFactory(new NullLogger<Zigbee2MqttMessageFactory>()));
        }

        [Fact]
        public void WhenInterviewSuccessfulMessageThenReturnsDeviceDiscovered()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .WithInterviewSuccessful()
                .WithPairingType()
                .WithMeta(meta => meta.WithFriendlyName("this-is-an-id"))
                .BuildMqttMessage();

            var result = _mapper.Map(message);

            Assert.Equal(HausEventTopic, result.Topic);
        }

        [Fact]
        public void WhenStateMessageThenReturnsUnknownMessage()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithStateTopic()
                .WithState("online")
                .BuildMqttMessage();

            var result = _mapper.Map(message);

            Assert.Equal(UnknownEventTopic, result.Topic);
        }

        [Fact]
        public void WhenFromSensorThenReturnsHausEvent()
        {
            var message = new Zigbee2MqttMessageBuilder(Zigbee2MqttTopic)
                .WithDeviceTopic("some-device-name")
                .WithIlluminance(4)
                .BuildMqttMessage();

            var result = _mapper.Map(message);
            
            Assert.Equal(HausEventTopic, result.Topic);
        }
    }
}