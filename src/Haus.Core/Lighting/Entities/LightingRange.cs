using System;

namespace Haus.Core.Lighting.Entities
{
    public abstract class LightingRange
    {
        public double Value { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }

        protected LightingRange(double value, double min, double max)
        {
            Value = value;
            Min = min;
            Max = max;
        }

        public double CalculateTargetValue(LightingRange target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            var targetValue = (target.Value * Max) / target.Max;
            return Math.Max(Min, targetValue);
        }
    }
}