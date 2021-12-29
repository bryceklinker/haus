using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting.Generators;

public class LeveLightingDefaultGenerator : IDefaultLightingGenerator
{
    private static readonly LightingEntity Default = new(
        LightingDefaults.State,
        new LevelLightingEntity()
    );

    public LightingEntity Generate(LightingEntity currentLighting, LightingEntity roomLighting)
    {
        var lighting = LightingEntity.FromEntity(currentLighting ?? roomLighting ?? Default);
        lighting.Color = null;
        lighting.Temperature = null;
        lighting.Level ??= Default.Level;
        return lighting;
    }
}