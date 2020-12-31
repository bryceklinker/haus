namespace Haus.Core.Models.Common
{
    public record LightingModel
    {
        public static readonly LightingModel Default = new();
        public LightingState State { get; }
        public double BrightnessPercent { get; }
        public double Temperature { get; }
        public LightingColorModel Color { get; }

        public LightingModel(
            LightingState state = LightingState.Off, 
            double brightnessPercent = 0, 
            double temperature = 0, 
            LightingColorModel color = null)
        {
            State = state;
            BrightnessPercent = brightnessPercent;
            Temperature = temperature;
            Color = color ?? LightingColorModel.Default;
        }
    }
}