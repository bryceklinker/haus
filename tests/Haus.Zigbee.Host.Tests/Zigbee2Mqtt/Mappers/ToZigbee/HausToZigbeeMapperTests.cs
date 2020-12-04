using System.Text;
using Haus.Core.Models.Devices.Discovery;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToZigbee
{
    public class HausToZigbeeMapperTests
    {
        private const string Zigbee2MqttBaseTopic = "something";
        private readonly HausToZigbeeMapper _mapper;

        public HausToZigbeeMapperTests()
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
            
            _mapper = new HausToZigbeeMapper(zigbeeOptions);
        }
        
        [Fact]
        public void WhenStartDiscoveryCommandReceivedThenReturnsPermitJoinTrueMessage()
        {
            var original = new StartDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original);

            Assert.Equal($"{Zigbee2MqttBaseTopic}/bridge/config/permit_join", result.Topic);
            Assert.Equal("true", Encoding.UTF8.GetString(result.Payload));
        }

        [Fact]
        public void WhenStopDiscoveryCommandReceivedThenReturnsPermitJoinFalseMessage()
        {
            var original = new StopDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original);

            Assert.Equal($"{Zigbee2MqttBaseTopic}/bridge/config/permit_join", result.Topic);
            Assert.Equal("false", Encoding.UTF8.GetString(result.Payload));
        }
    }
}