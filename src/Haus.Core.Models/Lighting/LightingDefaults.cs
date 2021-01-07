namespace Haus.Core.Models.Lighting
{
    public static class LightingDefaults
    {
        public const LightingState State = LightingState.Off;

        public const double MinTemperature = 2000;
        public const double Temperature = 3000;
        public const double MaxTemperature = 6000;
        
        public const double MinLevel = 0;
        public const double Level = 100;
        public const double MaxLevel = 100;
        
        public const byte Red = 255;
        public const byte Green = 255;
        public const byte Blue = 255;
    }
}