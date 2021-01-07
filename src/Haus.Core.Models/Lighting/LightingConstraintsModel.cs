namespace Haus.Core.Models.Lighting
{
    public record LightingConstraintsModel(
        double MinLevel = LightingDefaults.MinLevel, 
        double MaxLevel = LightingDefaults.MaxLevel,
        double MinTemperature = LightingDefaults.MinTemperature,
        double MaxTemperature = LightingDefaults.MaxTemperature)
    {
        public static readonly LightingConstraintsModel Default = new();
    }
}