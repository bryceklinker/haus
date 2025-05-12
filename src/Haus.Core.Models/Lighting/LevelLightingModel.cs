namespace Haus.Core.Models.Lighting;

public record LevelLightingModel(
    double Value = LightingDefaults.Level,
    double Min = LightingDefaults.MinLevel,
    double Max = LightingDefaults.MaxLevel
) : LightingRangeModel(Value, Min, Max);
