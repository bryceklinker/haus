namespace Haus.Core.Models.Lighting;

public record LightingModel(
    LightingState State = LightingDefaults.State,
    LevelLightingModel? Level = null,
    TemperatureLightingModel? Temperature = null,
    ColorLightingModel? Color = null
)
{
    public LevelLightingModel Level { get; init; } = Level ?? new LevelLightingModel();
}
