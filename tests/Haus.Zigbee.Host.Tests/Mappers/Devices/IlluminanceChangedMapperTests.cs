using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers.Devices
{
    public class IlluminanceChangedMapperTests
    {
        private readonly IlluminanceChangedMapper _mapper;

        public IlluminanceChangedMapperTests()
        {
            _mapper = new IlluminanceChangedMapper();
        }

        [Fact]
        public void WhenIlluminanceChangedThenReturnsPopulatedIlluminanceChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithDeviceTopic("1231")
                .WithIlluminance(65)
                .WithIlluminanceLux(12)
                .BuildZigbee2MqttMessage();

            var model = _mapper.Map(message);

            Assert.Equal(65, model.Illuminance);
            Assert.Equal(12, model.Lux);
            Assert.Equal("1231", model.DeviceId);
        }

        [Fact]
        public void WhenIlluminanceNotReportedThenReturnsNull()
        {
            var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

            Assert.Null(_mapper.Map(message));
        }
    }
}