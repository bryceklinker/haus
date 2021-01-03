namespace Haus.Core.Models.Lighting
{
    public record LightingConstraintsModel(
        double MinLevel = LightingConstraintsModel.DefaultMinLevel, 
        double MaxLevel = LightingConstraintsModel.DefaultMaxLevel,
        double MinTemperature = LightingConstraintsModel.DefaultMinTemperature,
        double MaxTemperature = LightingConstraintsModel.DefaultMaxTemperature)
    {
        public const double DefaultMinLevel = 0;
        public const double DefaultMaxLevel = 100;
        public const double DefaultMinTemperature = 2000;
        public const double DefaultMaxTemperature = 6000;
        
        public static readonly LightingConstraintsModel Default = new();
    }
}