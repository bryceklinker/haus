namespace Haus.Core.Models.Lighting
{
    public record LightingConstraintsModel(
        double MinBrightnessValue = LightingConstraintsModel.DefaultMinBrightnessValue, 
        double MaxBrightnessValue = LightingConstraintsModel.DefaultMaxBrightnessValue,
        double MinTemperature = LightingConstraintsModel.DefaultMinTemperature,
        double MaxTemperature = LightingConstraintsModel.DefaultMaxTemperature)
    {
        public const double DefaultMinBrightnessValue = 0;
        public const double DefaultMaxBrightnessValue = 100;
        public const double DefaultMinTemperature = 2000;
        public const double DefaultMaxTemperature = 6000;
    }
}