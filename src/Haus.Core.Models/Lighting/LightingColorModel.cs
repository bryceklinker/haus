namespace Haus.Core.Models.Lighting
{
    public record LightingColorModel(byte Red = LightingDefaults.Red, byte Green = LightingDefaults.Green, byte Blue = LightingDefaults.Blue)
    {
        public static readonly LightingColorModel Default = new();
    }
}