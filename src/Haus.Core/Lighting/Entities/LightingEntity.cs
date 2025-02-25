using System;
using System.Linq.Expressions;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities;

public record LightingEntity(
    LightingState State = LightingDefaults.State,
    LevelLightingEntity? Level = null,
    TemperatureLightingEntity? Temperature = null,
    ColorLightingEntity? Color = null
)
{
    public LightingState State { get; set; } = State;
    public LevelLightingEntity? Level { get; set; } = Level;
    public TemperatureLightingEntity? Temperature { get; set; } = Temperature;
    public ColorLightingEntity? Color { get; set; } = Color;

    public LightingEntity()
        : this(LightingDefaults.State) { }

    public static readonly Expression<Func<LightingEntity, LightingModel>> ToModelExpression = l => new LightingModel(
        l.State,
        l.Level == null ? null : new LevelLightingModel(l.Level.Value, l.Level.Min, l.Level.Max),
        l.Temperature == null
            ? null
            : new TemperatureLightingModel(l.Temperature.Value, l.Temperature.Min, l.Temperature.Max),
        l.Color == null ? null : new ColorLightingModel(l.Color.Red, l.Color.Green, l.Color.Blue)
    );

    private static readonly Lazy<Func<LightingEntity, LightingModel>> ToModelFunc = new(ToModelExpression.Compile);

    public LightingModel ToModel()
    {
        return ToModelFunc.Value(this);
    }

    public LightingEntity? CalculateTarget(LightingEntity? target)
    {
        return CalculateTarget(this, target);
    }

    public LightingEntity? ConvertToConstraints(LightingConstraintsModel model)
    {
        var level = new LevelLightingEntity(0, model.MinLevel, model.MaxLevel);
        var temperature =
            model.HasTemperature && Temperature != null
                ? new TemperatureLightingEntity(0, model.MinTemperature.Value, model.MaxTemperature.Value)
                : null;
        var lighting = this with { Level = level, Temperature = temperature };
        return lighting.CalculateTarget(this);
    }

    public static LightingEntity FromModel(LightingModel model)
    {
        return new LightingEntity(
            model.State,
            LevelLightingEntity.FromModel(model.Level),
            TemperatureLightingEntity.FromModel(model.Temperature),
            ColorLightingEntity.FromModel(model.Color)
        );
    }

    public static LightingEntity? FromEntity(LightingEntity? entity)
    {
        if (entity == null)
            return null;

        return new LightingEntity(
            entity.State,
            LevelLightingEntity.FromEntity(entity.Level),
            TemperatureLightingEntity.FromEntity(entity.Temperature),
            ColorLightingEntity.FromEntity(entity.Color)
        );
    }

    public static LightingEntity? CalculateTarget(LightingEntity? current, LightingEntity? target)
    {
        if (current == null)
            return target;

        if (target == null)
            return current;

        var level = LevelLightingEntity.CalculateTarget(current.Level, target.Level);
        var temperature = TemperatureLightingEntity.CalculateTarget(current.Temperature, target.Temperature);
        var color = ColorLightingEntity.CalculateTarget(current.Color, target.Color);
        return new LightingEntity(target.State, level, temperature, color);
    }

    public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, LightingEntity> builder)
        where TEntity : class
    {
        builder.Property(l => l.State).HasConversion<string>().IsRequired();
        builder.OwnsOne(l => l.Level, LevelLightingEntity.Configure);
        builder.OwnsOne(l => l.Temperature, TemperatureLightingEntity.Configure);
        builder.OwnsOne(l => l.Color, ColorLightingEntity.Configure);
    }
}
