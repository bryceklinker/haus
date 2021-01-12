using System.Drawing;
using FluentAssertions;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.Lighting.Entities
{
    public class LightingEntityTests
    {
        [Fact]
        public void WhenCreatedFromModelThenLightingIsPopulatedFromModel()
        {
            var model = new LightingModel(
                LightingState.On,
                new LevelLightingModel(43.12, 10, 90),
                new TemperatureLightingModel(78, 0, 1000),
                new ColorLightingModel(12, 3, 6)
            );

            var lighting = LightingEntity.FromModel(model);

            lighting.Should().BeEquivalentTo(new LightingEntity(
                    LightingState.On,
                    new LevelLightingEntity(43.12, 10, 90),
                    new TemperatureLightingEntity(78, 0, 1000),
                    new ColorLightingEntity(12, 3, 6)
                )
            );
        }

        [Fact]
        public void
            WhenLightingTurnedIntoDesiredLightingThenLevelIsCalculatedBasedOnConstraintsOfTheCurrentLightingAndDesiredLighting()
        {
            const double desiredLevel = 45;
            const double desiredMaxLevelConstraint = 100;
            const double currentMaxLevelConstraint = 2000;

            var current = new LightingEntity(level: new LevelLightingEntity(12, 0, currentMaxLevelConstraint));
            var desired =
                new LightingEntity(level: new LevelLightingEntity(desiredLevel, max: desiredMaxLevelConstraint));

            var result = current.CalculateTarget(desired);

            const double expected = (desiredLevel * currentMaxLevelConstraint) / desiredMaxLevelConstraint;
            result.Level.Should().BeEquivalentTo(new LevelLightingEntity(expected, 0, currentMaxLevelConstraint));
        }

        [Fact]
        public void
            WhenLightingTurnedIntoDesiredLightingThenTemperatureIsCalculatedBasedOnConstraintsOfTheCurrentLightingAndDesiredLighting()
        {
            const double desiredTemperature = 5000;
            const double desiredMaxTemperatureConstraint = 8000;
            const double currentMaxTemperatureConstraint = 250;

            var current =
                new LightingEntity(temperature: new TemperatureLightingEntity(0, 0, currentMaxTemperatureConstraint));
            var desired =
                new LightingEntity(temperature: new TemperatureLightingEntity(desiredTemperature, 0,
                    desiredMaxTemperatureConstraint));

            var result = current.CalculateTarget(desired);

            const double expected = (desiredTemperature * currentMaxTemperatureConstraint) /
                                    desiredMaxTemperatureConstraint;
            result.Temperature.Should()
                .BeEquivalentTo(new TemperatureLightingEntity(expected, 0, currentMaxTemperatureConstraint));
        }

        [Fact]
        public void
            WhenTargetLightingLevelIsCalculatedToBeBelowMinimumLevelThenReturnsMinimumLevelFromCurrentLighting()
        {
            var current = new LightingEntity(level: new LevelLightingEntity(87, 87, 100));
            var desired = new LightingEntity(level: new LevelLightingEntity(50, 0, 100));

            var result = current.CalculateTarget(desired);

            result.Level.Should().BeEquivalentTo(new LevelLightingEntity(87, 87, 100));
        }

        [Fact]
        public void WhenCalculatingTargetLightingThenLightingRangeIsNotModified()
        {
            var current = new LightingEntity(level: new LevelLightingEntity(100, 100, 1000));
            var desired = new LightingEntity(level: new LevelLightingEntity(0, 0, 2000));

            var result = current.CalculateTarget(desired);

            result.Level.Should().BeEquivalentTo(new LevelLightingEntity(100, 100, 1000));
        }

        [Fact]
        public void WhenCurrentLightingIsMissingTemperatureAndTargetHasTemperatureThenReturnsLightingMissingTemperature()
        {
            var current = new LightingEntity(level: new LevelLightingEntity(45));
            var target = new LightingEntity(level: new LevelLightingEntity(65), temperature: new TemperatureLightingEntity());

            var result = current.CalculateTarget(target);

            result.Temperature.Should().BeNull();
        }

        [Fact]
        public void WhenCurrentLightingIsMissingColorAndTargetHasColorThenReturnsLightingMissingColor()
        {
            var current = new LightingEntity(level: new LevelLightingEntity(45));
            var target = new LightingEntity(level: new LevelLightingEntity(65), color: new ColorLightingEntity());

            var result = current.CalculateTarget(target);

            result.Color.Should().BeNull();
        }

        [Fact]
        public void WhenCalculatingTargetLightingThenReturnsNewLightingInstance()
        {
            var current = new LightingEntity();
            var desired = new LightingEntity();

            var result = current.CalculateTarget(desired);

            result.Should().NotBeSameAs(current);
        }

        [Fact]
        public void WhenLightingConvertedToRangeWithLevelOnlyThenLevelIsConvertedToNewRange()
        {
            var current = new LightingEntity(level: new LevelLightingEntity(50));
            var model = new LightingConstraintsModel(1, 251);

            var converted = current.ConvertToConstraints(model);

            converted.Level.Value.Should().Be(125.5);
            converted.Level.Min.Should().Be(1);
            converted.Level.Max.Should().Be(251);
        }

        [Fact]
        public void WhenLightingConvertedToRangeWithTemperatureThenTemperatureIsConvertedToNewRange()
        {
            var current = new LightingEntity(temperature: new TemperatureLightingEntity(4500));
            var model = new LightingConstraintsModel(0, 100, 1, 254);

            var converted = current.ConvertToConstraints(model);

            converted.Temperature.Value.Should().Be(190.5);
            converted.Temperature.Min.Should().Be(1);
            converted.Temperature.Max.Should().Be(254);
        }

        [Fact]
        public void WhenLightingConvertedToRangeThenReturnsColorUnchanged()
        {
            var current = new LightingEntity(
                level: new LevelLightingEntity(50),
                color: new ColorLightingEntity(123, 123, 123));
            
            var model = new LightingConstraintsModel(200, 500);

            var converted = current.ConvertToConstraints(model);

            converted.Color.Should().BeEquivalentTo(new ColorLightingEntity(123, 123, 123));
        }
    }
}