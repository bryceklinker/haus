namespace Haus.Core.Tests.Lighting.Entities
{
    public record ValueRange(double Min, double Max)
    {
        public double GetValueWithinRange(double value)
        {
            var result = value;
            if (result < Min) result = Min;
            if (result > Max) result = Max;
            return result;
        }
    }
}