using Haus.Core.Models.Sensors;
using Haus.Core.Models.Sensors.Battery;
using Haus.Core.Models.Sensors.Light;
using Haus.Core.Models.Sensors.Motion;
using Haus.Core.Models.Sensors.Temperature;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers.Devices
{
    public class SensorChangedMapperTests
    {
        private readonly SensorChangedMapper _mapper;

        public SensorChangedMapperTests()
        {
            _mapper = new SensorChangedMapper();
        }

        [Fact]
        public void WhenMessageIsForLightSensorThenReturnsIlluminanceChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithIlluminance(54)
                .BuildZigbee2MqttMessage();

            Assert.IsType<IlluminanceChangedModel>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForMotionSensorThenReturnsOccupancyChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithOccupancy(true)
                .BuildZigbee2MqttMessage();

            Assert.IsType<OccupancyChangedModel>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForTemperatureSensorThenReturnsTemperatureChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithTemperature(43)
                .BuildZigbee2MqttMessage();

            Assert.IsType<TemperatureChangedModel>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForBatterySensorThenReturnsBatteryChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithBatteryLevel(5)
                .BuildZigbee2MqttMessage();

            Assert.IsType<BatteryChangedModel>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForBatteryAndIlluminanceThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithBatteryLevel(45)
                .WithIlluminance(2)
                .BuildZigbee2MqttMessage();

            Assert.IsType<MultiSensorChanged>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForBatteryAndOccupancyThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithBatteryLevel(5)
                .WithOccupancy(true)
                .BuildZigbee2MqttMessage();

            Assert.IsType<MultiSensorChanged>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForBatteryAndTemperatureThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithBatteryLevel(5)
                .WithTemperature(1)
                .BuildZigbee2MqttMessage();

            Assert.IsType<MultiSensorChanged>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForIlluminanceAndOccupancyThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithIlluminance(3)
                .WithOccupancy(false)
                .BuildZigbee2MqttMessage();

            Assert.IsType<MultiSensorChanged>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForIlluminanceAndTemperatureThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithIlluminance(6)
                .WithTemperature(12)
                .BuildZigbee2MqttMessage();

            Assert.IsType<MultiSensorChanged>(_mapper.Map(message));
        }

        [Fact]
        public void WhenMessageIsForOccupancyAndTemperatureThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithOccupancy(false)
                .WithTemperature(12)
                .BuildZigbee2MqttMessage();
            
            Assert.IsType<MultiSensorChanged>(_mapper.Map(message));
        }
    }
}