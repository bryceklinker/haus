using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using MQTTnet;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToZigbee
{
    public class HausLightingToZigbeeMapperTests
    {
        private readonly HausLightingToZigbeeMapper _mapper;
        private readonly DeviceModel _device;

        public HausLightingToZigbeeMapperTests()
        {
            var zigbeeOptions = OptionsFactory.CreateZigbeeOptions("basy");
            _device = new DeviceModel {ExternalId = $"{Guid.NewGuid()}"};
            _mapper = new HausLightingToZigbeeMapper(zigbeeOptions);
        }

        [Fact]
        public void WhenTypeIsDeviceLightingChangedThenIsSupported()
        {
            Assert.True(_mapper.IsSupported(DeviceLightingChangedEvent.Type));
        }

        [Fact]
        public void WhenTypeIsNotDeviceLightingChangedThenUnsupported()
        {
            Assert.False(_mapper.IsSupported("anything else"));
        }

        [Fact]
        public void WhenMappedThenTopicIsSetDevice()
        {
            var original = CreateMqttMessage(new LightingModel());

            var message = _mapper.Map(original).Single();

            Assert.Equal($"basy/{_device.ExternalId}/set", message.Topic);
        }

        [Fact]
        public void WhenLightingModelIsFullyPopulatedThenZigbeeLightingIsPopulated()
        {
            var lightingModel = new LightingModel(
                LightingState.On,
                new LevelLightingModel(54),
                new TemperatureLightingModel(67),
                new ColorLightingModel(98, 54, 234));

            var message = _mapper.Map(CreateMqttMessage(lightingModel)).Single();

            var result = JObject.Parse(Encoding.UTF8.GetString(message.Payload));
            result.Value<string>("state").Should().Be("ON");
            result.Value<int>("brightness").Should().Be(54);
            result.Value<int>("color_temp").Should().Be(67);
            result.Value<JObject>("color").Value<int>("b").Should().Be(234);
            result.Value<JObject>("color").Value<int>("g").Should().Be(54);
            result.Value<JObject>("color").Value<int>("r").Should().Be(98);
        }

        [Fact]
        public void WhenLightingIsMissingTemperatureThenColorTempIsMissingFromPayload()
        {
            var lighting = new LightingModel(
                LightingState.On,
                new LevelLightingModel(12, 0, 254)
            );
            
            var message = _mapper.Map(CreateMqttMessage(lighting)).Single();
            
            var result = JObject.Parse(Encoding.UTF8.GetString(message.Payload));
            result.TryGetValue("color_temp", out _).Should().BeFalse();
        }

        [Fact]
        public void WhenLightingIsMissingColorThenColorIsMissingFromPayload()
        {
            var lighting = new LightingModel(
                LightingState.On,
                new LevelLightingModel(12, 0, 254)
            );
            
            var message = _mapper.Map(CreateMqttMessage(lighting)).Single();
            
            var result = JObject.Parse(Encoding.UTF8.GetString(message.Payload));
            result.TryGetValue("color", out _).Should().BeFalse();
        }

        [Fact]
        public void WhenLightingModelMissingColorThenReturnsZigbeeLighting()
        {
            var model = new LightingModel(LightingState.On);

            var original = CreateMqttMessage(model);
            var message = _mapper.Map(original).Single();

            var result = JObject.Parse(Encoding.UTF8.GetString(message.Payload));
            Assert.Equal("ON", result.Value<string>("state"));
        }

        [Fact]
        public void WhenLightingModelHasOffStateThenReturnsLightingWithStateOnly()
        {
            var lightingModel = new LightingModel(
                LightingState.Off,
                new LevelLightingModel(54),
                new TemperatureLightingModel(67),
                new ColorLightingModel(98, 54, 234));
            
            var message = _mapper.Map(CreateMqttMessage(lightingModel)).Single();

            var result = JObject.Parse(Encoding.UTF8.GetString(message.Payload));
            result.Value<string>("state").Should().Be("OFF");
            result.TryGetValue("brightness", out _).Should().BeFalse();
            result.TryGetValue("color_temp", out _).Should().BeFalse();
            result.TryGetValue("color", out _).Should().BeFalse();
        }
        
        private MqttApplicationMessage CreateMqttMessage(LightingModel lighting)
        {
            return new DeviceLightingChangedEvent(_device, lighting)
                .AsHausCommand()
                .ToMqttMessage("haus/commands");
        }
    }
}