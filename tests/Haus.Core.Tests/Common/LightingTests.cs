using Haus.Core.Common;
using Haus.Core.Models.Common;
using Xunit;

namespace Haus.Core.Tests.Common
{
    public class LightingTests
    {
        [Fact]
        public void DefaultLightingHasPopulatedLightingObject()
        {
            Assert.Equal(LightingState.Off, Lighting.Default.State);
            Assert.Equal(100, Lighting.Default.Brightness);
            Assert.Equal(150, Lighting.Default.Temperature);
            Assert.Equal(255, Lighting.Default.Color.Blue);
            Assert.Equal(255, Lighting.Default.Color.Green);
            Assert.Equal(255, Lighting.Default.Color.Red);
        }
        
        [Fact]
        public void WhenCreatedFromModelThenLightingIsPopulatedFromModel()
        {
            var model = CreateLightingModel();

            var lighting = Lighting.FromModel(model);

            Assert.Equal(LightingState.On, lighting.State);
            Assert.Equal(78, lighting.Temperature);
            Assert.Equal(43.12, lighting.Brightness);
            Assert.Equal((byte)6, lighting.Color.Blue);
            Assert.Equal((byte)3, lighting.Color.Green);
            Assert.Equal((byte)12, lighting.Color.Red);
        }

        [Fact]
        public void WhenLightingIsCopiedThenNewLightingReturned()
        {
            var lighting = Lighting.FromModel(CreateLightingModel());

            var copy = lighting.Copy();

            Assert.NotSame(lighting, copy);
            Assert.NotSame(lighting.Color, copy.Color);
            Assert.Equal(lighting.State, copy.State);
            Assert.Equal(lighting.Temperature, copy.Temperature);
            Assert.Equal(lighting.Brightness, copy.Brightness);
            Assert.Equal(lighting.Color.Blue, copy.Color.Blue);
            Assert.Equal(lighting.Color.Green, copy.Color.Green);
            Assert.Equal(lighting.Color.Red, copy.Color.Red);
        }

        [Fact]
        public void WhenComparingLightingsThenValuesAreUsedToCompare()
        {
            var first = Lighting.FromModel(CreateLightingModel());
            var second = Lighting.FromModel(CreateLightingModel());

            Assert.True(first.Equals(second));
        }

        private static LightingModel CreateLightingModel()
        {
            return new LightingModel
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
        }
    }
}