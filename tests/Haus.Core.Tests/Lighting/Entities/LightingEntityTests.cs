using FluentAssertions;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.Lighting.Entities
{
    public class LightingEntityTests
    {
        [Fact]
        public void DefaultLightingHasPopulatedLightingObject()
        {
            LightingEntity.Default.Should().BeEquivalentTo(new LightingEntity
            {
                State = LightingState.Off,
                Level = 100,
                Temperature = 3000,
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
                Level = 43.12,
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

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void WhenLightingTurnedIntoDesiredLightingThenLevelIsCalculatedBasedOnConstraintsOfTheCurrentLightingAndDesiredLighting()
        {
            const double desiredLevel = 45;
            const double desiredMaxLevelConstraint = 100;
            const double currentMaxLevelConstraint = 2000;
            
            var current = new LightingEntity(constraints: new LightingConstraintsEntity(90, currentMaxLevelConstraint));
            var desired = new LightingEntity(level: desiredLevel, constraints: new LightingConstraintsEntity(0, desiredMaxLevelConstraint));

            var result = current.ToDesiredLighting(desired);

            const double expected = (desiredLevel * currentMaxLevelConstraint) / desiredMaxLevelConstraint; 
            result.Level.Should().Be(expected);
        }

        [Fact]
        public void WhenLightingTurnedIntoDesiredLightingThenTemperatureIsCalculatedBasedOnConstraintsOfTheCurrentLightingAndDesiredLighting()
        {
            const double desiredTemperature = 5000;
            const double desiredMaxTemperatureConstraint = 6000;
            const double currentMaxTemperatureConstraint = 250;
            
            var current = new LightingEntity(constraints: new LightingConstraintsEntity(minTemperature: 0, maxTemperature: currentMaxTemperatureConstraint));
            var desired = new LightingEntity(temperature: desiredTemperature, constraints: new LightingConstraintsEntity(minTemperature: 0, maxTemperature: desiredMaxTemperatureConstraint));

            var result = current.ToDesiredLighting(desired);
            
            const double expected = (desiredTemperature * currentMaxTemperatureConstraint) / desiredMaxTemperatureConstraint;
            result.Temperature.Should().Be(expected);
        }

        [Fact]
        public void WhenDesiredLightingLevelIsCalculatedToBeBelowMinimumLevelThenReturnsMinimumLevelFromCurrentLighting()
        {
            var current = new LightingEntity(constraints: new LightingConstraintsEntity(87, 100));
            var desired = new LightingEntity(level: 50, constraints: new LightingConstraintsEntity(0, 100));

            var result = current.ToDesiredLighting(desired);

            result.Level.Should().Be(87);
        }

        [Fact]
        public void WhenConvertingToDesiredLevelThenConstraintsAreNotModified()
        {
            var current = new LightingEntity(constraints: new LightingConstraintsEntity(100, 1000));
            var desired = new LightingEntity(constraints: new LightingConstraintsEntity(0, 2000));

            var result = current.ToDesiredLighting(desired);

            result.Constraints.MinLevel.Should().Be(100);
            result.Constraints.MaxLevel.Should().Be(1000);
        }

        [Fact]
        public void WhenConvertingToDesiredLevelThenReturnsNewLightingInstance()
        {
            var current = new LightingEntity();
            var desired = new LightingEntity();

            var result = current.ToDesiredLighting(desired);

            result.Should().NotBeSameAs(current);
        }

        [Fact]
        public void WhenLightingConstraintsAreChangedThenReturnsLightingWithNewConstraints()
        {
            var current = new LightingEntity();

            var constraints = new LightingConstraintsEntity(25, 75, 0, 250);
            var result = current.ChangeLightingConstraints(constraints);

            result.Should().NotBeSameAs(current);
            result.Constraints.Should().BeEquivalentTo(constraints);
        }

        [Fact]
        public void WhenLightingConstraintsMaxValuesAreChangedBelowCurrentValuesThenReturnsLightingAtNewMaxValues()
        {
            var current = new LightingEntity(level: 90, temperature: 6000);
            var constraints = new LightingConstraintsEntity(25, 75, 0, 250);

            var result = current.ChangeLightingConstraints(constraints);

            result.Level.Should().Be(75);
            result.Temperature.Should().Be(250);
        }

        [Fact]
        public void WhenLightingConstraintsMinValuesAreChangedAboveCurrentValuesThenReturnsLightingAtNewMinValues()
        {
            var current = new LightingEntity(level: 25, temperature: 10);
            var constraints = new LightingConstraintsEntity(30, 75, 70, 250);

            var result = current.ChangeLightingConstraints(constraints);

            result.Level.Should().Be(30);
            result.Temperature.Should().Be(70);
        }
        
        private static LightingModel CreateLightingModel()
        {
            var color = new LightingColorModel(12, 3, 6);
            return new LightingModel(LightingState.On, 43.12, 78, color);
        }
    }
}