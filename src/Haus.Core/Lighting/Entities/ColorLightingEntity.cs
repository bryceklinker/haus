using System;
using FluentValidation.Internal;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities
{
    public class ColorLightingEntity
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public ColorLightingEntity()
            : this(LightingDefaults.Red)
        {
        }

        public ColorLightingEntity(
            byte red = LightingDefaults.Red, 
            byte green = LightingDefaults.Green,
            byte blue = LightingDefaults.Blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        protected bool Equals(ColorLightingEntity other)
        {
            return Red == other.Red && Green == other.Green && Blue == other.Blue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ColorLightingEntity) obj);
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
            builder.Property(c => c.Red);
            builder.Property(c => c.Green);
            builder.Property(c => c.Blue);
        }
    }
}