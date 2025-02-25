using Haus.Core.Lighting.Entities;

namespace Haus.Core.Lighting.Generators;

public class NonLightLightingDefaultGenerator : IDefaultLightingGenerator
{
    public LightingEntity Generate(LightingEntity currentLighting, LightingEntity roomLighting)
    {
        return null;
    }
}
