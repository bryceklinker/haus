using System;
using System.Linq;
using System.Text;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
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
            _device = new DeviceModel{ExternalId = $"{Guid.NewGuid()}"};
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
            var lightingModel = new LightingModel
            {
                State = LightingState.Off,
                Brightness = 54,
                Temperature = 67,
                Color = new LightingColorModel
                {
                    Blue = 234,
                    Green = 54,
                    Red = 98
                }
            };

            var original = CreateMqttMessage(lightingModel);
            var message = _mapper.Map(original).Single();

            var result = JObject.Parse(Encoding.UTF8.GetString(message.Payload));
            Assert.Equal("OFF", result.Value<string>("state"));
            Assert.Equal(54, result.Value<int>("brightness"));
            Assert.Equal(67, result.Value<int>("color_temp"));
            Assert.Equal(234, result.Value<JObject>("color").Value<int>("b"));
            Assert.Equal(54, result.Value<JObject>("color").Value<int>("g"));
            Assert.Equal(98, result.Value<JObject>("color").Value<int>("r"));
        }

        [Fact]
        public void WhenLightingModelMissingColorThenReturnsZigbeeLighting()
        {
            var model = new LightingModel{ State = LightingState.On};

            var original = CreateMqttMessage(model);
            var message = _mapper.Map(original).Single();

            var result = JObject.Parse(Encoding.UTF8.GetString(message.Payload));
            Assert.Equal("ON", result.Value<string>("state"));
        }

        private MqttApplicationMessage CreateMqttMessage(LightingModel lighting)
        {
            return new DeviceLightingChangedEvent(_device, lighting)
                .AsHausCommand()
                .ToMqttMessage("haus/commands");
        }
    }
}