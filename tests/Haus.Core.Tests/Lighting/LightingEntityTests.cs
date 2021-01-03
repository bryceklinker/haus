using FluentAssertions;
using Haus.Core.Lighting;
using Haus.Core.Models.Common;
using Xunit;

namespace Haus.Core.Tests.Lighting
{
    public class LightingEntityTests
    {
        [Fact]
        public void DefaultLightingHasPopulatedLightingObject()
        {
            LightingEntity.Default.Should().BeEquivalentTo(new LightingEntity
            {
                State = LightingState.Off,
                BrightnessPercent = 100,
                Temperature = 150,
                Color = new LightingColorEntity
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

            var lighting = LightingEntity.FromModel(model);
            
            lighting.Should().BeEquivalentTo(new LightingEntity
            {
                State = LightingState.On,
                Temperature = 78,
                BrightnessPercent = 43.12,
                Color = new LightingColorEntity
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
            var lighting = LightingEntity.FromModel(CreateLightingModel());

            var copy = lighting.Copy();

            copy.Should().BeEquivalentTo(lighting);
            copy.Should().NotBeSameAs(lighting);
        }

        [Fact]
        public void WhenComparingLightingsThenValuesAreUsedToCompare()
        {
            var first = LightingEntity.FromModel(CreateLightingModel());
            var second = LightingEntity.FromModel(CreateLightingModel());

            Assert.True(first.Equals(second));
        }

        private static LightingModel CreateLightingModel()
        {
            var color = new LightingColorModel(12, 3, 6);
            return new LightingModel(LightingState.On, 43.12, 78, color);
        }
    }
}