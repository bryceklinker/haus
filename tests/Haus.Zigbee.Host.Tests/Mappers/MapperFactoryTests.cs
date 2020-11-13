using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers
{
    public class MapperFactoryTests
    {
        private const string ZigbeeTopicName = "idk";
        private readonly MapperFactory _mapperFactory;
        
        public MapperFactoryTests()
        {
            var zigbeeOptions = Options.Create(new ZigbeeOptions
            {
                Config = new Zigbee2MqttConfiguration
                {
                    Mqtt = new MqttConfiguration
                    {
                        BaseTopic = ZigbeeTopicName
                    }
                }
            });
            _mapperFactory = new MapperFactory(Options.Create(new HausOptions()), zigbeeOptions);
        }

        [Fact]
        public void WhenTopicIsBridgeTopicThenReturnsBridgeMapper()
        {
            var mapper = _mapperFactory.GetMapper($"{ZigbeeTopicName}/bridge/log");
            Assert.IsType<BridgeMessageMapper>(mapper);
        }

        [Fact]
        public void WhenTopicIsNotRecognizedThenReturnsUnknownMapper()
        {
            var mapper = _mapperFactory.GetMapper("not-recognized");
            Assert.IsType<UnknownMessageMapper>(mapper);
        }
    }
}