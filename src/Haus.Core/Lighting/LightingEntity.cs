using System;
using System.Linq.Expressions;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting
{
    public class LightingEntity
    {
        private const LightingState DefaultState = LightingState.Off;
        private const double DefaultTemperature = 150;
        private const double DefaultLevel = 100;
        public static readonly LightingEntity Default = new();

        public LightingState State { get; set; }
        public double Level { get; set; }
        public double Temperature { get; set; }
        public LightingColorEntity Color { get; set; }
        public LightingConstraintsEntity Constraints { get; set; }

        public LightingEntity()
            : this(DefaultState, DefaultLevel, DefaultTemperature, LightingColorEntity.Default, LightingConstraintsEntity.Default)
        {
            
        }
        
        public LightingEntity(
            LightingState state = DefaultState, 
            double level = DefaultLevel,
            double temperature = DefaultTemperature,
            LightingColorEntity color = null,
            LightingConstraintsEntity constraints = null)
        {
            State = state;
            Level = level;
            Temperature = temperature;
            Color = color ?? LightingColorEntity.Default.Copy();
            Constraints = constraints ?? LightingConstraintsEntity.Default.Copy();
        }
        
        public static readonly Expression<Func<LightingEntity, LightingModel>> ToModelExpression = 
            l => new LightingModel(
                l.State, 
                l.Level, 
                l.Temperature, 
                new LightingColorModel(l.Color.Red, l.Color.Green, l.Color.Blue),
                new LightingConstraintsModel(l.Constraints.MinLevel, l.Constraints.MaxLevel, l.Constraints.MinTemperature, l.Constraints.MaxTemperature));

        private static readonly Lazy<Func<LightingEntity, LightingModel>> ToModelFunc = new(ToModelExpression.Compile);
        
        public LightingModel ToModel()
        {
            return ToModelFunc.Value(this);
        }

        public LightingEntity ToDesiredLighting(LightingEntity desired)
        {
            var actual = Copy();
            actual.Level = CalculateDesiredLevel(desired);
            actual.Temperature = desired.Temperature;
            actual.State = desired.State;
            actual.Color = desired.Color.Copy();
            return actual;
        }

        public LightingEntity Copy()
        {
            return new(State, Level, Temperature, Color.Copy(), Constraints.Copy());
        }

        public static LightingEntity FromModel(LightingModel model)
        {
            return new(
                model.State,
                model.Level,
                model.Temperature,
                LightingColorEntity.FromModel(model.Color),
                LightingConstraintsEntity.FromModel(model.Constraints));
        }

        public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, LightingEntity> builder) 
            where TEntity : class
        {
            builder.Property(l => l.State).HasConversion<string>();
            builder.OwnsOne(l => l.Color);
            builder.OwnsOne(l => l.Constraints, LightingConstraintsEntity.Configure);
        }

        private double CalculateDesiredLevel(LightingEntity desired)
        {
            var desiredLevel = (desired.Level * Constraints.MaxLevel) / desired.Constraints.MaxLevel;
            return Math.Max(desiredLevel, Constraints.MinLevel);
        }

        protected bool Equals(LightingEntity other)
        {
            return State == other.State 
                   && Nullable.Equals(Level, other.Level) 
                   && Nullable.Equals(Temperature, other.Temperature) 
                   && Equals(Color, other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LightingEntity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State, Level, Temperature, Color);
        }
    }
}