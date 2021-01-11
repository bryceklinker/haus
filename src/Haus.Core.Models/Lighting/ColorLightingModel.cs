namespace Haus.Core.Models.Lighting
{
    public record ColorLightingModel(byte Red = LightingDefaults.Red, byte Green = LightingDefaults.Green, byte Blue = LightingDefaults.Blue)
    {
    }
}