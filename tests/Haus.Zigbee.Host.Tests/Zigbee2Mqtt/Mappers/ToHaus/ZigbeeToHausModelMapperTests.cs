using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus
{
    public class ZigbeeToHausModelMapperTests
    {
        private const string Zigbee2MqttTopic = "zigbee2mqtt";
        private const string HausEventTopic = "haus/events";
        private const string UnknownEventTopic = "haus/unknown";
        private readonly ZigbeeToHausMapper _mapper;

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
            var hausOptions = new OptionsMonitorFake<HausOptions>(new HausOptions
            {
                EventsTopic = HausEventTopic,
                UnknownTopic = UnknownEventTopic
            });

            _mapper = new ZigbeeToHausMapper(
                hausOptions, 
                zigbeeOptions,
                new Zigbee2MqttMessageFactory(new NullLogger<Zigbee2MqttMessageFactory>()));
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