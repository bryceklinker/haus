using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Devices;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.Devices
{
    public class TemperatureChangedMapperTests
    {
        private readonly TemperatureChangedMapper _mapper;

        public TemperatureChangedMapperTests()
        {
            _mapper = new TemperatureChangedMapper();
        }

        [Fact]
        public void WhenTemperatureChangedThenReturnsPopulatedTemperatureChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithTemperature(65)
                .WithDeviceTopic("1234")
                .BuildZigbee2MqttMessage();

            var model = _mapper.Map(message);

            Assert.Equal("1234", model.DeviceId);
            Assert.Equal(65, model.Temperature);
        }

        [Fact]
        public void WhenTemperatureNotReportedThenReturnsNull()
        {
            var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

            Assert.Null(_mapper.Map(message));
        }
    }
}