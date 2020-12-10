using System.Text;
using Haus.Core.Models.Common;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToZigbee
{
    public class HausLightingToZigbeeMapperTests
    {
        private readonly HausLightingToZigbeeMapper _mapper;

        public HausLightingToZigbeeMapperTests()
        {
            _mapper = new HausLightingToZigbeeMapper();
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
            
            var bytes = _mapper.Map(lightingModel);

            var result = JObject.Parse(Encoding.UTF8.GetString(bytes));
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

            var bytes = _mapper.Map(model);

            var result = JObject.Parse(Encoding.UTF8.GetString(bytes));
            Assert.Equal("ON", result.Value<string>("state"));
        }
    }
}