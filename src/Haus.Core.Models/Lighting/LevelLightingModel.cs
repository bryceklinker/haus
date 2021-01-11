namespace Haus.Core.Models.Lighting
{
    public record LevelLightingModel(
        double Value = LightingDefaults.Level,
        double MinLevel = LightingDefaults.MinLevel,
        double MaxLevel = LightingDefaults.MaxLevel);
}