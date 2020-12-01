using Haus.Core.Models;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Devices;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.Devices
{
    public class DeviceMessageMapperTests
    {
        private const string EventsTopicName = "idk";
        private readonly DeviceMessageMapper _mapper;

        public DeviceMessageMapperTests()
        {
            _mapper = new DeviceMessageMapper(new OptionsMonitorFake<HausOptions>(new HausOptions
            {
                EventsTopic = EventsTopicName
            }));
        }

        [Fact]
        public void WhenFromMultiSensorThenEventIsMultiSensorChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
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
            var message = new Zigbee2MqttMessageBuilder()
                .WithTemperature(54)
                .WithDeviceTopic("my-device-id")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);
            
            AssertHausEventTypeIs(TemperatureChangedModel.Type, result);
        }

        [Fact]
        public void WhenFromLightSensorThenEventIsIlluminanceChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithIlluminance(3)
                .WithDeviceTopic("the-id")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);

            AssertHausEventTypeIs(IlluminanceChangedModel.Type, result);
        }

        [Fact]
        public void WhenFromMotionSensorThenEventIsOccupancyChanged()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithOccupancy(true)
                .WithDeviceTopic("idk")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);

            AssertHausEventTypeIs(OccupancyChangedModel.Type, result);
        }

        [Fact]
        public void WhenFromBatterySensorThenEventIsBatteryChangedEvent()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithBatteryLevel(54)
                .WithDeviceTopic("one")
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);

            AssertHausEventTypeIs(BatteryChangedModel.Type, result);
        }

        private static void AssertHausEventTypeIs(string expectedType, MqttApplicationMessage result)
        {
            var payload = HausJsonSerializer.Deserialize<HausEvent>(result.Payload);
            Assert.Equal(expectedType, payload.Type);
        }
    }
}