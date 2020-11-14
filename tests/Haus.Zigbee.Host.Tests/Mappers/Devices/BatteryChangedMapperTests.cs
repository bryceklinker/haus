using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers.Devices
{
    public class BatteryChangedMapperTests
    {
        private readonly BatteryChangedMapper _mapper;

        public BatteryChangedMapperTests()
        {
            _mapper = new BatteryChangedMapper();
        }

        [Fact]
        public void WhenBatteryChangedThenReturnsPopulatedBatteryChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithBatteryLevel(43)
                .WithFriendlyName("my-device-id")
                .BuildZigbee2MqttMessage();

            var model = _mapper.Map(message);

            Assert.Equal(43, model.BatteryLevel);
            Assert.Equal("my-device-id", model.DeviceId);
        }

        [Fact]
        public void WhenBatteryLevelNotReportedThenReturnsNull()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder().BuildZigbee2MqttMessage();

            Assert.Null(_mapper.Map(message));
        }
    }
}