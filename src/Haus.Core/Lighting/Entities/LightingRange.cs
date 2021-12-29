using System;

namespace Haus.Core.Lighting.Entities;

public abstract record LightingRange(double Value, double Min, double Max)
{
    public double Value { get; set; } = Value;
    public double Min { get; set; } = Min;
    public double Max { get; set; } = Max;

    public double CalculateTargetValue(LightingRange target)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        var targetValue = target.Value * Max / target.Max;
        return Math.Max(Min, targetValue);
    }
}