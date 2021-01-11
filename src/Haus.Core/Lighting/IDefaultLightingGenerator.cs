using Haus.Core.Lighting.Entities;

namespace Haus.Core.Lighting
{
    public interface IDefaultLightingGenerator
    {
        LightingEntity Generate(LightingEntity currentLighting, LightingEntity roomLighting);
    }
}