using System;
using System.Linq.Expressions;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting
{
    public class LightingConstraintsEntity
    {
        public const double DefaultMinBrightnessValue = 0;
        public const double DefaultMaxBrightnessValue = 100;
        public const double DefaultMinTemperature = 2000;
        public const double DefaultMaxTemperature = 6000;

        public static readonly LightingConstraintsEntity Default = new();

        public static readonly Expression<Func<LightingConstraintsEntity, LightingConstraintsModel>> ToModelExpression =
            e => new LightingConstraintsModel(
                e.MinBrightnessValue, 
                e.MaxBrightnessValue, 
                e.MinTemperature,
                e.MaxTemperature);
        
        public double MinBrightnessValue { get; set; } = DefaultMinBrightnessValue;
        public double MaxBrightnessValue { get; set; } = DefaultMaxBrightnessValue;
        public double MinTemperature { get; set; } = DefaultMinTemperature;
        public double MaxTemperature { get; set; } = DefaultMaxTemperature;

        public void UpdateFromModel(LightingConstraintsModel model)
        {
            MinBrightnessValue = model.MinBrightnessValue;
            MaxBrightnessValue = model.MaxBrightnessValue;
            MinTemperature = model.MinTemperature;
            MaxTemperature = model.MaxTemperature;
        }
        
        protected bool Equals(LightingConstraintsEntity other)
        {
            return MinBrightnessValue.Equals(other.MinBrightnessValue) && MaxBrightnessValue.Equals(other.MaxBrightnessValue) && MinTemperature.Equals(other.MinTemperature) && MaxTemperature.Equals(other.MaxTemperature);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LightingConstraintsEntity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MinBrightnessValue, MaxBrightnessValue, MinTemperature, MaxTemperature);
        }

        public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, LightingConstraintsEntity> builder)
            where TEntity : class
        {
            
        }
    }
}