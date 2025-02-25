using Haus.Core.Lighting.Entities;

namespace Haus.Core.Lighting.Generators;

public interface IDefaultLightingGenerator
{
    LightingEntity? Generate(LightingEntity? currentLighting, LightingEntity? roomLighting);
}
