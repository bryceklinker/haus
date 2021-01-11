namespace Haus.Core.Models.Lighting
{
    public record TemperatureLightingModel(
        double Value = LightingDefaults.Temperature,
        double Min = LightingDefaults.MinTemperature,
        double Max = LightingDefaults.MaxTemperature);
}