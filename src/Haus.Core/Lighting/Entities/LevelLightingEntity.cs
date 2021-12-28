using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities
{
    public record LevelLightingEntity : LightingRange
    {
        public LevelLightingEntity(
            double value = LightingDefaults.Level, 
            double min = LightingDefaults.MinLevel, 
            double max = LightingDefaults.MaxLevel)
            : base(value, min, max)
        {
            Value = value;
            Min = min;
            Max = max;
        }

        public static LevelLightingEntity CalculateTarget(LevelLightingEntity current, LevelLightingEntity target)
        {
            if (current == null)
                return null;
            if (target == null)
                return FromEntity(current);

            var value = current.CalculateTargetValue(target);
            return new LevelLightingEntity(value, current.Min, current.Max);
        }

        public static LevelLightingEntity FromModel(LevelLightingModel model)
        {
            if (model == null)
                return null;

            return new LevelLightingEntity(model.Value, model.Min, model.Max);
        }

        public static LevelLightingEntity FromEntity(LevelLightingEntity entity)
        {
            if (entity == null)
                return null;

            return new LevelLightingEntity(entity.Value, entity.Min, entity.Max);
        }

        public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, LevelLightingEntity> builder) 
            where TEntity : class
        {
            builder.Property(l => l.Value).IsRequired();
            builder.Property(l => l.Min).IsRequired();
            builder.Property(l => l.Max).IsRequired();
        }
    }
}