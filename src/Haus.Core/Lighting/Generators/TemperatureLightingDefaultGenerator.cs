using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting.Generators
{
    public class TemperatureLightingDefaultGenerator : IDefaultLightingGenerator
    {
        private static readonly LightingEntity Default = new(
            LightingDefaults.State,
            new LevelLightingEntity(),
            new TemperatureLightingEntity()
        );

        public LightingEntity Generate(LightingEntity currentLighting, LightingEntity roomLighting)
        {
            var lighting = LightingEntity.FromEntity(currentLighting ?? Default);
            lighting.Color = null;
            lighting.Temperature = Default.Temperature;
            lighting.Level ??= Default.Level;
            return roomLighting != null 
                ? lighting.CalculateTarget(roomLighting) 
                : lighting;
        }
    }
}