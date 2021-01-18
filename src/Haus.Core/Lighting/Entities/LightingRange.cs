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

        protected bool Equals(LightingRange other)
        {
            return Value.Equals(other.Value) && Min.Equals(other.Min) && Max.Equals(other.Max);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LightingRange) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Min, Max);
        }
    }
}