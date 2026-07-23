using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.Lighting.Entities;

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

        Assert.Equal(
            new LightingEntity(
                LightingState.On,
                new LevelLightingEntity(43.12, 10, 90),
                new TemperatureLightingEntity(78, 0, 1000),
                new ColorLightingEntity(12, 3, 6)
            ),
            lighting
        );
    }

    [Fact]
    public void WhenLightingTurnedIntoDesiredLightingThenLevelIsCalculatedBasedOnConstraintsOfTheCurrentLightingAndDesiredLighting()
    {
        const double desiredLevel = 45;
        const double desiredMaxLevelConstraint = 100;
        const double currentMaxLevelConstraint = 2000;

        var current = new LightingEntity(Level: new LevelLightingEntity(12, 0, currentMaxLevelConstraint));
        var desired = new LightingEntity(Level: new LevelLightingEntity(desiredLevel, max: desiredMaxLevelConstraint));

        var result = current.CalculateTarget(desired);

        const double expected = desiredLevel * currentMaxLevelConstraint / desiredMaxLevelConstraint;
        Assert.Equal(new LevelLightingEntity(expected, 0, currentMaxLevelConstraint), result?.Level);
    }

    [Fact]
    public void WhenLightingTurnedIntoDesiredLightingThenTemperatureIsCalculatedBasedOnConstraintsOfTheCurrentLightingAndDesiredLighting()
    {
        const double desiredTemperature = 5000;
        const double desiredMaxTemperatureConstraint = 8000;
        const double currentMaxTemperatureConstraint = 250;

        var current = new LightingEntity(
            Temperature: new TemperatureLightingEntity(0, 0, currentMaxTemperatureConstraint)
        );
        var desired = new LightingEntity(
            Temperature: new TemperatureLightingEntity(desiredTemperature, 0, desiredMaxTemperatureConstraint)
        );

        var result = current.CalculateTarget(desired);

        const double expected = desiredTemperature * currentMaxTemperatureConstraint / desiredMaxTemperatureConstraint;
        Assert.Equal(new TemperatureLightingEntity(expected, 0, currentMaxTemperatureConstraint), result?.Temperature);
    }

    [Fact]
    public void WhenTargetLightingLevelIsCalculatedToBeBelowMinimumLevelThenReturnsMinimumLevelFromCurrentLighting()
    {
        var current = new LightingEntity(Level: new LevelLightingEntity(87, 87));
        var desired = new LightingEntity(Level: new LevelLightingEntity(50));

        var result = current.CalculateTarget(desired);

        Assert.Equal(new LevelLightingEntity(87, 87), result?.Level);
    }

    [Fact]
    public void WhenCalculatingTargetLightingThenLightingRangeIsNotModified()
    {
        var current = new LightingEntity(Level: new LevelLightingEntity(100, 100, 1000));
        var desired = new LightingEntity(Level: new LevelLightingEntity(0, 0, 2000));

        var result = current.CalculateTarget(desired);

        Assert.Equal(new LevelLightingEntity(100, 100, 1000), result?.Level);
    }

    [Fact]
    public void WhenCurrentLightingIsMissingTemperatureAndTargetHasTemperatureThenReturnsLightingMissingTemperature()
    {
        var current = new LightingEntity(Level: new LevelLightingEntity(45));
        var target = new LightingEntity(
            Level: new LevelLightingEntity(65),
            Temperature: new TemperatureLightingEntity()
        );

        var result = current.CalculateTarget(target);

        Assert.Null(result?.Temperature);
    }

    [Fact]
    public void WhenCurrentLightingIsMissingColorAndTargetHasColorThenReturnsLightingMissingColor()
    {
        var current = new LightingEntity(Level: new LevelLightingEntity(45));
        var target = new LightingEntity(Level: new LevelLightingEntity(65), Color: new ColorLightingEntity());

        var result = current.CalculateTarget(target);

        Assert.Null(result?.Color);
    }

    [Fact]
    public void WhenCalculatingTargetLightingThenReturnsNewLightingInstance()
    {
        var current = new LightingEntity();
        var desired = new LightingEntity();

        var result = current.CalculateTarget(desired);

        Assert.NotSame(current, result);
    }

    [Fact]
    public void WhenLightingConvertedToRangeWithLevelOnlyThenLevelIsConvertedToNewRange()
    {
        var current = new LightingEntity(Level: new LevelLightingEntity(50));
        var model = new LightingConstraintsModel(1, 251);

        var converted = current.ConvertToConstraints(model);

        Assert.Equal(125.5, converted?.Level?.Value);
        Assert.Equal(1, converted?.Level?.Min);
        Assert.Equal(251, converted?.Level?.Max);
    }

    [Fact]
    public void WhenLightingConvertedToRangeWithTemperatureThenTemperatureIsConvertedToNewRange()
    {
        var current = new LightingEntity(Temperature: new TemperatureLightingEntity(4500));
        var model = new LightingConstraintsModel(0, 100, 1, 254);

        var converted = current.ConvertToConstraints(model);

        Assert.Equal(190.5, converted?.Temperature?.Value);
        Assert.Equal(1, converted?.Temperature?.Min);
        Assert.Equal(254, converted?.Temperature?.Max);
    }

    [Fact]
    public void WhenLightingConvertedToRangeThenReturnsColorUnchanged()
    {
        var current = new LightingEntity(
            Level: new LevelLightingEntity(50),
            Color: new ColorLightingEntity(123, 123, 123)
        );

        var model = new LightingConstraintsModel(200, 500);

        var converted = current.ConvertToConstraints(model);

        Assert.Equal(new ColorLightingEntity(123, 123, 123), converted?.Color);
    }
}
