using Haus.Core.Models.Devices.Discovery;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Factories;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers
{
    public class MqttMessageTranslatorTests
    {
        private const string Zigbee2MqttBaseTopic = "zigbee";
        private const string HausCommandsTopic = "some/commands";
        private const string HausEventsTopic = "other/events";
        private readonly MqttMessageMapper _mapper;

        public MqttMessageTranslatorTests()
        {
            var zigbeeOptions = Options.Create(new ZigbeeOptions
            {
                Config = new Zigbee2MqttConfiguration
                {
                    Mqtt = new MqttConfiguration
                    {
                        BaseTopic = Zigbee2MqttBaseTopic
                    }
                }
            });

            var hausOptions = new OptionsMonitorFake<HausOptions>(new HausOptions
            {
                EventsTopic = HausEventsTopic,
                CommandsTopic = HausCommandsTopic
            });

            var toHausModelMapper = new ZigbeeToHausMapper(
                hausOptions,
                zigbeeOptions,
                new Zigbee2MqttMessageFactory(new NullLogger<Zigbee2MqttMessageFactory>()));
            var toZigbeeMapper = new HausToZigbeeMapper(zigbeeOptions);
            _mapper = new MqttMessageMapper(hausOptions, zigbeeOptions, toHausModelMapper, toZigbeeMapper);
        }

        [Fact]
        public void WhenMessageIsFromZigbee2MqttTopicThenReturnsMessageWithHausEventsTopic()
        {
            var message = new Zigbee2MqttMessageBuilder(Zigbee2MqttBaseTopic)
                .WithLogTopic()
                .WithPairingType()
                .WithInterviewSuccessful()
                .WithMeta(meta => meta.WithFriendlyName("idk"))
                .BuildMqttMessage();
            
            var result = _mapper.Map(message);
            
            Assert.Equal(HausEventsTopic, result.Topic);
        }

        [Fact]
        public void WhenMessageIsFromHausThenReturnsMessageWithZigbeeTopic()
        {
            var message = new StartDiscoveryModel().AsHausCommand().ToMqttMessage(HausCommandsTopic);

            var result = _mapper.Map(message);

            Assert.StartsWith(Zigbee2MqttBaseTopic, result.Topic);
        }
        
        [Fact]
        public void WhenMessageIsFromIrrelevantTopicThenReturnsNull()
        {
            var message = new MqttApplicationMessage { Topic = "no-one-cares" };

            var result = _mapper.Map(message);
            
            Assert.Null(result);;
        }
    }
}