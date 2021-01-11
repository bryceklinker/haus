using System;
using System.Linq.Expressions;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities
{
    public class LightingEntity
    {
        public LightingState State { get; set; }
        public LevelLightingEntity Level { get; set; }
        public TemperatureLightingEntity Temperature { get; set; }
        public ColorLightingEntity Color { get; set; }

        public LightingEntity()
            : this(LightingDefaults.State)
        {
        }

        public LightingEntity(
            LightingState state = LightingDefaults.State,
            LevelLightingEntity level = null,
            TemperatureLightingEntity temperature = null,
            ColorLightingEntity color = null)
        {
            State = state;
            Level = level;
            Temperature = temperature;
            Color = color;
        }

        public static readonly Expression<Func<LightingEntity, LightingModel>> ToModelExpression =
            l => new LightingModel(
                l.State,
                l.Level == null ? null : new LevelLightingModel(l.Level.Value, l.Level.Min, l.Level.Max),
                l.Temperature == null ? null : new TemperatureLightingModel(l.Temperature.Value, l.Temperature.Min, l.Temperature.Max),
                l.Color == null ? null : new ColorLightingModel(l.Color.Red, l.Color.Green, l.Color.Blue)
            );

        private static readonly Lazy<Func<LightingEntity, LightingModel>> ToModelFunc = new(ToModelExpression.Compile);

        public LightingModel ToModel()
        {
            return ToModelFunc.Value(this);
        }

        public LightingEntity CalculateTarget(LightingEntity target)
        {
            return CalculateTarget(this, target);
        }

        public static LightingEntity FromModel(LightingModel model)
        {
            return new(
                model.State,
                LevelLightingEntity.FromModel(model.Level),
                TemperatureLightingEntity.FromModel(model.Temperature),
                ColorLightingEntity.FromModel(model.Color));
        }

        public static LightingEntity FromEntity(LightingEntity entity)
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

        public static LightingEntity CalculateTarget(LightingEntity current, LightingEntity target)
        {
            if (current == null)
                return null;
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
            builder.Property(l => l.State).HasConversion<string>();
            builder.OwnsOne(l => l.Level, LevelLightingEntity.Configure);
            builder.OwnsOne(l => l.Temperature, TemperatureLightingEntity.Configure);
            builder.OwnsOne(l => l.Color, ColorLightingEntity.Configure);
        }
    }
}