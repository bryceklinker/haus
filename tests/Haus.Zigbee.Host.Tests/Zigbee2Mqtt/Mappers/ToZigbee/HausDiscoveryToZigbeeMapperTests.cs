using System.Linq;
using System.Text;
using Haus.Core.Models.Devices.Discovery;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToZigbee
{
    public class HausDiscoveryToZigbeeMapperTests
    {
        private const string ZigbeeBaseTopic = "woot";
        private readonly HausDiscoveryToZigbeeMapper _mapper;

        public HausDiscoveryToZigbeeMapperTests()
        {
            var options = OptionsFactory.CreateZigbeeOptions(ZigbeeBaseTopic);
            _mapper = new HausDiscoveryToZigbeeMapper(options);
        }
        
        [Fact]
        public void WhenTypeIsStartDiscoveryThenIsSupported()
        {
            Assert.True(_mapper.IsSupported(StartDiscoveryModel.Type));
        }

        [Fact]
        public void WhenTypeIsStopDiscoveryThenIsSupported()
        {
            Assert.True(_mapper.IsSupported(StopDiscoveryModel.Type));
        }
        
        [Fact]
        public void WhenTypeIsSyncDiscoveryThenIsSupported()
        {
            Assert.True(_mapper.IsSupported(SyncDiscoveryModel.Type));
        }

        [Fact]
        public void WhenTypeIsNotADiscoveryTypeThenUnsupported()
        {
            Assert.False(_mapper.IsSupported("not discovery"));
        }

        [Fact]
        public void WhenStartDiscoveryIsMappedThenReturnsPermitJoinTrue()
        {
            var original = new StartDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original).Single();

            Assert.Equal($"{ZigbeeBaseTopic}/bridge/config/permit_join", result.Topic);
            Assert.Equal("true", Encoding.UTF8.GetString(result.Payload));
        }

        [Fact]
        public void WhenStopDiscoveryIsMappedThenReturnsPermitJoinFalse()
        {
            var original = new StopDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original).Single();

            Assert.Equal($"{ZigbeeBaseTopic}/bridge/config/permit_join", result.Topic);
            Assert.Equal("false", Encoding.UTF8.GetString(result.Payload));
        }
        
        [Fact]
        public void WhenSyncDiscoveryIsMappedThenReturnsGetDevices()
        {
            var original = new SyncDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original).Single();

            Assert.Equal($"{ZigbeeBaseTopic}/bridge/config/devices/get", result.Topic);
            Assert.Empty(result.Payload);
        }
    }
}