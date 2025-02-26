using System;

namespace Haus.Core.Lighting.Entities;

public abstract record LightingRange(double Value, double Min, double Max)
{
    public double CalculateTargetValue(LightingRange? target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var targetValue = target.Value * Max / target.Max;
        return Math.Max(Min, targetValue);
    }
}
