namespace Haus.Core.Models.Lighting;

public record LightingConstraintsModel(
    double MinLevel,
    double MaxLevel,
    double? MinTemperature = null,
    double? MaxTemperature = null
)
{
    [OptionalGeneration]
    public bool HasTemperature => MinTemperature.HasValue && MaxTemperature.HasValue;
}
