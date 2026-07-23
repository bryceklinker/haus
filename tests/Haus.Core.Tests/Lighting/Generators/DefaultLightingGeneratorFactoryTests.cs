using Haus.Core.Lighting.Generators;
using Haus.Core.Models.Devices;
using Xunit;

namespace Haus.Core.Tests.Lighting.Generators;

public class DefaultLightingGeneratorFactoryTests
{
    [Fact]
    public void WhenNotALightDeviceThenReturnsNonLightLightingDefaultGenerator()
    {
        var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Switch, LightType.Level);

        Assert.IsAssignableFrom<NonLightLightingDefaultGenerator>(generator);
    }

    [Fact]
    public void WhenIsLightAndLightTypeIsLevelThenReturnsLevelLightingDefaultGenerator()
    {
        var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.Level);

        Assert.IsAssignableFrom<LeveLightingDefaultGenerator>(generator);
    }

    [Fact]
    public void WhenIsLightAndLightTypeIsTemperatureThenReturnsTemperatureLightingDefaultGenerator()
    {
        var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.Temperature);

        Assert.IsAssignableFrom<TemperatureLightingDefaultGenerator>(generator);
    }

    [Fact]
    public void WhenIsLightAndLightTypeIsColorThenReturnsColorLightingDefaultGenerator()
    {
        var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.Color);

        Assert.IsAssignableFrom<ColorLightingDefaultGenerator>(generator);
    }

    [Fact]
    public void WhenIsLightAndLightTypeIsNoneThenReturnsLevelLightingDefaultGenerator()
    {
        var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.None);

        Assert.IsAssignableFrom<LeveLightingDefaultGenerator>(generator);
    }
}
