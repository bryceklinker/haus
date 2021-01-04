using System;
using System.Linq.Expressions;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting
{
    public class LightingConstraintsEntity
    {
        public const double DefaultMinLevel = 0;
        public const double DefaultMaxLevel = 100;
        public const double DefaultMinTemperature = 2000;
        public const double DefaultMaxTemperature = 6000;

        public static readonly LightingConstraintsEntity Default = new();

        public static readonly Expression<Func<LightingConstraintsEntity, LightingConstraintsModel>> ToModelExpression =
            e => new LightingConstraintsModel(
                e.MinLevel, 
                e.MaxLevel, 
                e.MinTemperature,
                e.MaxTemperature);
        
        public double MinLevel { get; set; }
        public double MaxLevel { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }

        public LightingConstraintsEntity()
            : this(DefaultMinLevel, DefaultMaxLevel, DefaultMinTemperature, DefaultMaxTemperature)
        {
            
        }
        
        public LightingConstraintsEntity(
            double minLevel = DefaultMinLevel, 
            double maxLevel = DefaultMaxLevel, 
            double minTemperature = DefaultMinTemperature, 
            double maxTemperature = DefaultMaxTemperature)
        {
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            MinTemperature = minTemperature;
            MaxTemperature = maxTemperature;
        }
        
        public void UpdateFromModel(LightingConstraintsModel model)
        {
            MinLevel = model.MinLevel;
            MaxLevel = model.MaxLevel;
            MinTemperature = model.MinTemperature;
            MaxTemperature = model.MaxTemperature;
        }

        public LightingConstraintsEntity Copy()
        {
            return new(MinLevel, MaxLevel, MinTemperature, MaxTemperature);
        }

        public static LightingConstraintsEntity FromModel(LightingConstraintsModel model)
        {
            return new(model.MinLevel, model.MaxLevel, model.MinTemperature, model.MaxTemperature);
        }

        protected bool Equals(LightingConstraintsEntity other)
        {
            return MinLevel.Equals(other.MinLevel) && MaxLevel.Equals(other.MaxLevel) && MinTemperature.Equals(other.MinTemperature) && MaxTemperature.Equals(other.MaxTemperature);
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
            return HashCode.Combine(MinLevel, MaxLevel, MinTemperature, MaxTemperature);
        }

        public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, LightingConstraintsEntity> builder)
            where TEntity : class
        {
            
        }
    }
}