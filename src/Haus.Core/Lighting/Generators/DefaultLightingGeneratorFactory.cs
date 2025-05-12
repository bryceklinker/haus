using Haus.Core.Models.Devices;

namespace Haus.Core.Lighting.Generators;

public static class DefaultLightingGeneratorFactory
{
    public static IDefaultLightingGenerator GetGenerator(DeviceType deviceType, LightType lightType)
    {
        if (deviceType != DeviceType.Light)
            return new NonLightLightingDefaultGenerator();

        return lightType switch
        {
            LightType.Temperature => new TemperatureLightingDefaultGenerator(),
            LightType.Color => new ColorLightingDefaultGenerator(),
            _ => new LeveLightingDefaultGenerator(),
        };
    }
}
