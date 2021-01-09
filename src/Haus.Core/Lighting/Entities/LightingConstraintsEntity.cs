using System;
using System.Linq.Expressions;
using Haus.Core.Models.Lighting;
using Haus.Core.Tests.Lighting.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities
{
    public class LightingConstraintsEntity
    {
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

        public ValueRange TemperatureRange => new(MinTemperature, MaxTemperature);
        public ValueRange LevelRange => new(MinLevel, MaxLevel);
        
        public LightingConstraintsEntity()
            : this(LightingDefaults.MinLevel)
        {
            
        }
        
        public LightingConstraintsEntity(
            double minLevel = LightingDefaults.MinLevel, 
            double maxLevel = LightingDefaults.MaxLevel, 
            double minTemperature = LightingDefaults.MinTemperature, 
            double maxTemperature = LightingDefaults.MaxTemperature)
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

        public LightingConstraintsModel ToModel()
        {
            return new(MinLevel, MaxLevel, MinTemperature, MaxTemperature);
        }
        
        public double GetTemperatureWithinConstraints(double temperature)
        {
            return TemperatureRange.GetValueWithinRange(temperature);
        }

        public double GetLevelWithinConstraints(double level)
        {
            return LevelRange.GetValueWithinRange(level);
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