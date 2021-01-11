using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting
{
    public class ColorLightingDefaultGenerator : IDefaultLightingGenerator
    {
        private static readonly LightingEntity Default = new(
            LightingDefaults.State,
            new LevelLightingEntity(),
            null,
            new ColorLightingEntity()
        );

        public LightingEntity Generate(LightingEntity currentLighting, LightingEntity roomLighting)
        {
            var lighting = LightingEntity.FromEntity(currentLighting ?? roomLighting ?? Default);
            lighting.Temperature = null;
            lighting.Color ??= Default.Color;
            lighting.Level ??= Default.Level;
            return lighting;
        }
    }
}