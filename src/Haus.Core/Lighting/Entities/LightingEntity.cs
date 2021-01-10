using System;
using System.Linq.Expressions;
using Haus.Core.Models.Lighting;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting.Entities
{
    public class LightingEntity
    {
        public static readonly LightingEntity Default = new();

        public LightingState State { get; set; }
        public double Level { get; set; }
        public double Temperature { get; set; }
        public LightingColorEntity Color { get; set; }
        public LightingConstraintsEntity Constraints { get; set; }
        public double MaxLevel => Constraints.MaxLevel;
        public double MaxTemperature => Constraints.MaxTemperature;

        public LightingEntity()
            : this(LightingDefaults.State, LightingDefaults.Level, LightingDefaults.Temperature,
                LightingColorEntity.Default, LightingConstraintsEntity.Default)
        {
        }

        public LightingEntity(
            LightingState state = LightingDefaults.State,
            double level = LightingDefaults.Level,
            double temperature = LightingDefaults.Temperature,
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
                new LightingConstraintsModel(
                    l.Constraints.MinLevel,
                    l.Constraints.MaxLevel,
                    l.Constraints.MinTemperature,
                    l.Constraints.MaxTemperature
                )
            );

        private static readonly Lazy<Func<LightingEntity, LightingModel>> ToModelFunc = new(ToModelExpression.Compile);

        public LightingModel ToModel()
        {
            return ToModelFunc.Value(this);
        }

        public LightingEntity ToDesiredLighting(LightingEntity desired)
        {
            var actual = Copy();
            actual.Level = CalculateDesiredLevel(desired);
            actual.Temperature = CalculateDesiredTemperature(desired);
            actual.State = desired.State;
            actual.Color = desired.Color.Copy();
            return actual;
        }

        public LightingEntity ChangeLightingConstraints(LightingConstraintsEntity constraints)
        {
            var actual = Copy();
            actual.Constraints = constraints.Copy();
            return actual.ToDesiredLighting(actual);
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
            var desiredLevel = ConvertDesiredValueToEquivalentValue(desired.Level, MaxLevel, desired.MaxLevel);
            return Constraints.GetLevelWithinConstraints(desiredLevel);
        }

        private double CalculateDesiredTemperature(LightingEntity desired)
        {
            var desiredTemperature =
                ConvertDesiredValueToEquivalentValue(desired.Temperature, MaxTemperature, desired.MaxTemperature);
            return Constraints.GetTemperatureWithinConstraints(desiredTemperature);
        }

        private double ConvertDesiredValueToEquivalentValue(double desiredValue, double currentMax, double desiredMax)
        {
            return (desiredValue * currentMax) / desiredMax;
        }

        protected bool Equals(LightingEntity other)
        {
            return State == other.State
                   && Nullable.Equals(Level, other.Level)
                   && Nullable.Equals(Temperature, other.Temperature)
                   && Nullable.Equals(Constraints, other.Constraints)
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
            return HashCode.Combine(State, Level, Temperature, Color, Constraints);
        }
    }
}