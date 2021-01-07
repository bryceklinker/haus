namespace Haus.Core.Models.Lighting
{
    public record LightingModel
    {
        public static readonly LightingModel Default = new();
        public LightingState State { get; }
        public double Level { get; }
        public double Temperature { get; }
        public LightingColorModel Color { get; }
        public LightingConstraintsModel Constraints { get; }

        public LightingModel(
            LightingState state = LightingDefaults.State, 
            double level = LightingDefaults.Level,
            double temperature = LightingDefaults.Temperature, 
            LightingColorModel color = null,
            LightingConstraintsModel constraints = null)
        {
            State = state;
            Level = level;
            Temperature = temperature;
            Color = color ?? LightingColorModel.Default;
            Constraints = constraints ?? LightingConstraintsModel.Default;
        }
    }
}