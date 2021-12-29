namespace Haus.Core.Models.Lighting;

public record LightingConstraintsModel(
    double MinLevel,
    double MaxLevel,
    double? MinTemperature = null,
    double? MaxTemperature = null)
{
    [OptionalGeneration] public double? MinTemperature { get; } = MinTemperature;

    [OptionalGeneration] public double? MaxTemperature { get; } = MaxTemperature;

    [OptionalGeneration] public bool HasTemperature => MinTemperature.HasValue && MaxTemperature.HasValue;
}