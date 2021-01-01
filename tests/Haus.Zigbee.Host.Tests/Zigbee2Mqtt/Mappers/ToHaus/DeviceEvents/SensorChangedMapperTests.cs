using FluentAssertions;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.DeviceEvents
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
            var message = new Zigbee2MqttMessageBuilder()
                .WithIlluminance(54)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<IlluminanceChangedModel>();
        }

        [Fact]
        public void WhenMessageIsForMotionSensorThenReturnsOccupancyChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithOccupancy(true)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<OccupancyChangedModel>();
        }

        [Fact]
        public void WhenMessageIsForTemperatureSensorThenReturnsTemperatureChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithTemperature(43)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<TemperatureChangedModel>();
        }

        [Fact]
        public void WhenMessageIsForBatterySensorThenReturnsBatteryChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithBatteryLevel(5)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<BatteryChangedModel>();
        }

        [Fact]
        public void WhenMessageIsForBatteryAndIlluminanceThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithBatteryLevel(45)
                .WithIlluminance(2)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<MultiSensorChanged>();
        }

        [Fact]
        public void WhenMessageIsForBatteryAndOccupancyThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithBatteryLevel(5)
                .WithOccupancy(true)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<MultiSensorChanged>();
        }

        [Fact]
        public void WhenMessageIsForBatteryAndTemperatureThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithBatteryLevel(5)
                .WithTemperature(1)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<MultiSensorChanged>();
        }

        [Fact]
        public void WhenMessageIsForIlluminanceAndOccupancyThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithIlluminance(3)
                .WithOccupancy(false)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<MultiSensorChanged>();
        }

        [Fact]
        public void WhenMessageIsForIlluminanceAndTemperatureThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithIlluminance(6)
                .WithTemperature(12)
                .BuildZigbee2MqttMessage();

            _mapper.Map(message).Should().BeOfType<MultiSensorChanged>();
        }

        [Fact]
        public void WhenMessageIsForOccupancyAndTemperatureThenReturnsMultiSensorChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithOccupancy(false)
                .WithTemperature(12)
                .BuildZigbee2MqttMessage();
            
            _mapper.Map(message).Should().BeOfType<MultiSensorChanged>();
        }
    }
}