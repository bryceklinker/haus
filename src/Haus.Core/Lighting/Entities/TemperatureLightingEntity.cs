using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities;

public record TemperatureLightingEntity : LightingRange
{
    public TemperatureLightingEntity()
        : this(LightingDefaults.Temperature)
    {
    }

    public TemperatureLightingEntity(
        double value = LightingDefaults.Temperature,
        double min = LightingDefaults.MinTemperature,
        double max = LightingDefaults.MaxTemperature)
        : base(value, min, max)
    {
        Value = value;
        Min = min;
        Max = max;
    }

    public static TemperatureLightingEntity CalculateTarget(
        TemperatureLightingEntity current,
        TemperatureLightingEntity target)
    {
        if (current == null)
            return null;

        if (target == null)
            return FromEntity(current);

        var value = current.CalculateTargetValue(target);
        return new TemperatureLightingEntity(value, current.Min, current.Max);
    }

    public static TemperatureLightingEntity FromModel(TemperatureLightingModel model)
    {
        if (model == null)
            return null;

        return new TemperatureLightingEntity(model.Value, model.Min, model.Max);
    }

    public static TemperatureLightingEntity FromEntity(TemperatureLightingEntity entity)
    {
        if (entity == null)
            return null;

        return new TemperatureLightingEntity(entity.Value, entity.Min, entity.Max);
    }

    public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, TemperatureLightingEntity> builder)
        where TEntity : class
    {
        builder.Property(t => t.Value).IsRequired();
        builder.Property(t => t.Min).IsRequired();
        builder.Property(t => t.Max).IsRequired();
    }
}