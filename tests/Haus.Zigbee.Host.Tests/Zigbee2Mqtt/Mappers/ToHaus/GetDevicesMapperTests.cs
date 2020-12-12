using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus
{
    public class GetDevicesMapperTests
    {
        private readonly GetDevicesMapper _mapper;

        public GetDevicesMapperTests()
        {
            var zigbeeOptions = OptionsFactory.CreateZigbeeOptions();
            var hausOptions = OptionsFactory.CreateHausOptions();
            _mapper = new GetDevicesMapper(zigbeeOptions, hausOptions, new DeviceTypeResolver(hausOptions));
        }

        [Fact]
        public void WhenTopicIsGetDevicesThenIsSupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithGetDevicesTopic()
                .BuildZigbee2MqttMessage();
            
            Assert.True(_mapper.IsSupported(message));
        }

        [Fact]
        public void WhenTopicIsNotGetDevicesThenUnsupported()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithLogTopic()
                .BuildZigbee2MqttMessage();

            Assert.False(_mapper.IsSupported(message));
        }

        [Fact]
        public void WhenOneDeviceIsInGetDevicesMessageThenReturnsOneDeviceDiscoveredMessage()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithGetDevicesTopic()
                .WithDevice(device =>
                {
                    device.Add("friendly_name", "boom");
                })
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message).ToArray();

            Assert.Single(result);
            Assert.Equal(Defaults.HausOptions.EventsTopic, result.Single().Topic);
        }

        [Fact]
        public void WhenOneDeviceIsInGetDeviceMessagesThen()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithGetDevicesTopic()
                .WithDevice(device =>
                {
                    device.Add("friendly_name", "hello");
                    device.Add("description", "my desc");
                    device.Add("model", "65");
                    device.Add("vendor", "76");
                    device.Add("powerSource", "Battery");
                })
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message).Single();

            var @event = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredModel>>(result.Payload);
            Assert.Equal(DeviceDiscoveredModel.Type, @event.Type);
            Assert.Equal("hello", @event.Payload.Id);
            Assert.Equal("65", @event.Payload.Model);
            Assert.Equal("76", @event.Payload.Vendor);
            Assert.Equal("my desc", @event.Payload.Description);
            Assert.Equal(DeviceType.Unknown, @event.Payload.DeviceType);
            Assert.Contains(@event.Payload.Metadata, meta => meta.Key == "powerSource" && meta.Value == "Battery");
        }

        [Fact]
        public void WhenDeviceIsMappedThenDeviceTypeIsResolved()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithGetDevicesTopic()
                .WithDevice(device =>
                {
                    device.Add("model", "929002335001");
                    device.Add("vendor", "Philips");
                })
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message).Single();
            
            var @event = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredModel>>(result.Payload);
            Assert.Equal(DeviceType.Light, @event.Payload.DeviceType);
        }

        [Fact]
        public void WhenMultipleDevicesAreInMessageThenReturnsMultipleDiscoveredEvents()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithGetDevicesTopic()
                .WithDevice()
                .WithDevice()
                .WithDevice()
                .BuildZigbee2MqttMessage();

            var result = _mapper.Map(message);
            
            Assert.Equal(3, result.Count());
        }
    }
}