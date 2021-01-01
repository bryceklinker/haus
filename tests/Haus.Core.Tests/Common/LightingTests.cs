using FluentAssertions;
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
            Lighting.Default.Should().BeEquivalentTo(new Lighting
            {
                State = LightingState.Off,
                BrightnessPercent = 100,
                Temperature = 150,
                Color = new LightingColor
                {
                    Blue = 255,
                    Green = 255,
                    Red = 255,
                }
            });
        }
        
        [Fact]
        public void WhenCreatedFromModelThenLightingIsPopulatedFromModel()
        {
            var model = CreateLightingModel();

            var lighting = Lighting.FromModel(model);
            
            lighting.Should().BeEquivalentTo(new Lighting
            {
                State = LightingState.On,
                Temperature = 78,
                BrightnessPercent = 43.12,
                Color = new LightingColor
                {
                    Blue = 6,
                    Green = 3,
                    Red = 12
                }
            });
        }

        [Fact]
        public void WhenLightingIsCopiedThenNewLightingReturned()
        {
            var lighting = Lighting.FromModel(CreateLightingModel());

            var copy = lighting.Copy();

            copy.Should().BeEquivalentTo(lighting);
            copy.Should().NotBeSameAs(lighting);
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
            var color = new LightingColorModel(12, 3, 6);
            return new LightingModel(LightingState.On, 43.12, 78, color);
        }
    }
}