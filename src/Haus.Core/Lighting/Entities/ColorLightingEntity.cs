using System;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities;

public record ColorLightingEntity(byte Red = LightingDefaults.Red,
    byte Green = LightingDefaults.Green,
    byte Blue = LightingDefaults.Blue)
{
    public byte Red { get; } = Red;
    public byte Green { get; } = Green;
    public byte Blue { get; } = Blue;

    public ColorLightingEntity()
        : this(LightingDefaults.Red)
    {
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Red, Green, Blue);
    }

    public static ColorLightingEntity FromModel(ColorLightingModel model)
    {
        if (model == null)
            return null;

        return new ColorLightingEntity(model.Red, model.Green, model.Blue);
    }

    public static ColorLightingEntity FromEntity(ColorLightingEntity entity)
    {
        if (entity == null)
            return null;

        return new ColorLightingEntity(entity.Red, entity.Green, entity.Blue);
    }

    public static ColorLightingEntity CalculateTarget(ColorLightingEntity current, ColorLightingEntity target)
    {
        if (current == null)
            return null;

        if (target == null)
            return FromEntity(current);

        return new ColorLightingEntity(target.Red, target.Green, target.Blue);
    }

    public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, ColorLightingEntity> builder)
        where TEntity : class
    {
        builder.Property(c => c.Red).IsRequired();
        builder.Property(c => c.Green).IsRequired();
        builder.Property(c => c.Blue).IsRequired();
    }
}