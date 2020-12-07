using Haus.Core.Common;
using Haus.Core.Models.Common;
using Xunit;

namespace Haus.Core.Tests.Common
{
    public class LightingTests
    {
        [Fact]
        public void WhenCreatedFromModelThenLightingIsPopulatedFromModel()
        {
            var model = new LightingModel
            {
                State = LightingState.On,
                Temperature = 78,
                Brightness = 43.12,
                Color = new LightingColorModel
                {
                    Blue = 6,
                    Green = 3,
                    Red = 12
                }
            };

            var lighting = Lighting.FromModel(model);

            Assert.Equal(LightingState.On, lighting.State);
            Assert.Equal(78, lighting.Temperature);
            Assert.Equal(43.12, lighting.Brightness);
            Assert.Equal((byte)6, lighting.Color.Blue);
            Assert.Equal((byte)3, lighting.Color.Green);
            Assert.Equal((byte)12, lighting.Color.Red);
        }
    }
}