namespace Haus.Core.Models.Common
{
    public class LightingModel
    {
        public LightingState State { get; set; }
        public double BrightnessPercent { get; set; }
        public double Temperature { get; set; }
        public LightingColorModel Color { get; set; }
    }
}