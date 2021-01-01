using System.Linq;
using FluentAssertions;
using Haus.Core.Models.Devices.Discovery;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Microsoft.Extensions.DependencyInjection;
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
            var config = ConfigurationFactory.CreateConfig(
                zigbeeBaseTopic:Zigbee2MqttBaseTopic,
                hausEventsTopic: HausEventsTopic,
                hausCommandsTopic: HausCommandsTopic);
            var provider = ServiceProviderFactory.Create(config);

            var toHausModelMapper = provider.GetRequiredService<IZigbeeToHausMapper>();
            var toZigbeeMapper = provider.GetRequiredService<IHausToZigbeeMapper>();
            var hausOptions = provider.GetRequiredService<IOptionsMonitor<HausOptions>>();
            var zigbeeOptions = provider.GetRequiredService<IOptions<ZigbeeOptions>>();
            
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
            
            var result = _mapper.Map(message).Single();

            result.Topic.Should().Be(HausEventsTopic);
        }

        [Fact]
        public void WhenMessageIsFromHausThenReturnsMessageWithZigbeeTopic()
        {
            var message = new StartDiscoveryModel().AsHausCommand().ToMqttMessage(HausCommandsTopic);

            var result = _mapper.Map(message).Single();

            result.Topic.Should().StartWith(Zigbee2MqttBaseTopic);
        }
        
        [Fact]
        public void WhenMessageIsFromIrrelevantTopicThenReturnsNull()
        {
            var message = new MqttApplicationMessage { Topic = "no-one-cares" };

            var result = _mapper.Map(message);

            result.Should().BeEmpty();
        }
    }
}