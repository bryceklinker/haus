using System.Text.Json;
using Haus.Core.Models;
using Haus.Core.Models.Sensors;
using Haus.Core.Models.Sensors.Battery;
using Haus.Core.Models.Sensors.Light;
using Haus.Core.Models.Sensors.Motion;
using Haus.Core.Models.Sensors.Temperature;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.Devices;
using Microsoft.Extensions.Options;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers.Devices
{
    public class DeviceMessageMapperTests
    {
        private const string EventsTopicName = "idk";
        private readonly DeviceMessageMapper _mapper;

        public DeviceMessageMapperTests()
        {
            _mapper = new DeviceMessageMapper(Options.Create(new HausOptions
            {
                EventsTopic = EventsTopicName
            }));
        }

        [Fact]
        public void WhenFromMultiSensorThenEventIsMultiSensorChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithIlluminance(34)
                .WithOccupancy(true)
                .WithTemperature(42)
                .WithBatteryLevel(54)
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);

            AssertHausEventTypeIs(MultiSensorChanged.Type, result);
        }

        [Fact]
        public void WhenFromTemperatureSensorThenEventIsTemperatureChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithTemperature(54)
                .WithFriendlyName("my-device-id")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);
            
            AssertHausEventTypeIs(TemperatureChangedModel.Type, result);
        }

        [Fact]
        public void WhenFromLightSensorThenEventIsIlluminanceChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithIlluminance(3)
                .WithFriendlyName("the-id")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);

            AssertHausEventTypeIs(IlluminanceChangedModel.Type, result);
        }

        [Fact]
        public void WhenFromMotionSensorThenEventIsOccupancyChanged()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithOccupancy(true)
                .WithFriendlyName("idk")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);

            AssertHausEventTypeIs(OccupancyChangedModel.Type, result);
        }

        [Fact]
        public void WhenFromBatterySensorThenEventIsBatteryChangedEvent()
        {
            var message = new Zigbee2MqttDeviceMessageBuilder()
                .WithBatteryLevel(54)
                .WithFriendlyName("one")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);

            AssertHausEventTypeIs(BatteryChangedModel.Type, result);
        }

        private static void AssertHausEventTypeIs(string expectedType, MqttApplicationMessage result)
        {
            var payload = JsonSerializer.Deserialize<HausEvent>(result.Payload);
            Assert.Equal(expectedType, payload.Type);
        }
    }
}