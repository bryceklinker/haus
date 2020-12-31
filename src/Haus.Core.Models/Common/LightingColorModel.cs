namespace Haus.Core.Models.Common
{
    public record LightingColorModel(byte Red = 0, byte Green = 0, byte Blue = 0)
    {
        public static readonly LightingColorModel Default = new();
    }
}