using FluentAssertions;
using Haus.Core.Lighting.Generators;
using Haus.Core.Models.Devices;
using Xunit;

namespace Haus.Core.Tests.Lighting.Generators
{
    public class DefaultLightingGeneratorFactoryTests
    {
        [Fact]
        public void WhenNotALightDeviceThenReturnsNonLightLightingDefaultGenerator()
        {
            var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Switch, LightType.Level);

            generator.Should().BeAssignableTo<NonLightLightingDefaultGenerator>();
        }

        [Fact]
        public void WhenIsLightAndLightTypeIsLevelThenReturnsLevelLightingDefaultGenerator()
        {
            var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.Level);

            generator.Should().BeAssignableTo<LeveLightingDefaultGenerator>();
        }

        [Fact]
        public void WhenIsLightAndLightTypeIsTemperatureThenReturnsTemperatureLightingDefaultGenerator()
        {
            var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.Temperature);

            generator.Should().BeAssignableTo<TemperatureLightingDefaultGenerator>();
        }

        [Fact]
        public void WhenIsLightAndLightTypeIsColorThenReturnsColorLightingDefaultGenerator()
        {
            var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.Color);

            generator.Should().BeAssignableTo<ColorLightingDefaultGenerator>();
        }

        [Fact]
        public void WhenIsLightAndLightTypeIsNoneThenReturnsLevelLightingDefaultGenerator()
        {
            var generator = DefaultLightingGeneratorFactory.GetGenerator(DeviceType.Light, LightType.None);

            generator.Should().BeAssignableTo<LeveLightingDefaultGenerator>();
        }
    }
}