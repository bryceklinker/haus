using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Devices;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.Devices
{
    public class OccupancyChangedMapperTests
    {
        private readonly OccupancyChangedMapper _mapper;

        public OccupancyChangedMapperTests()
        {
            _mapper = new OccupancyChangedMapper();
        }

        [Fact]
        public void WhenOccupancyChangedThenReturnsPopulatedOccupancyChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithDeviceTopic("motions")
                .WithOccupancy(true)
                .WithOccupancyTimeout(123)
                .WithMotionSensitivity("low")
                .BuildZigbee2MqttMessage();

            var model = _mapper.Map(message);

            Assert.Equal("motions", model.DeviceId);
            Assert.True(model.Occupancy);
            Assert.Equal(123, model.Timeout);
            Assert.Equal("low", model.Sensitivity);
        }

        [Fact]
        public void WhenOccupancyNotReportedThenReturnsNull()
        {
            var message = new Zigbee2MqttMessageBuilder().BuildZigbee2MqttMessage();

            Assert.Null(_mapper.Map(message));
        }
    }
}