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
            LightingState state = LightingState.Off, 
            double level = 0,
            double temperature = 0, 
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