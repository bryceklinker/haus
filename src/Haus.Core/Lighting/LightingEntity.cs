using System;
using System.Linq.Expressions;
using Haus.Core.Models.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Lighting
{
    public class LightingEntity
    {
        private const LightingState DefaultState = LightingState.Off;
        private const double DefaultTemperature = 150;
        private const double DefaultBrightnessPercent = 100;
        public static readonly LightingEntity Default = new()
        {
            State = DefaultState,
            BrightnessPercent = DefaultBrightnessPercent,
            Color = LightingColorEntity.Default,
            Temperature = DefaultTemperature
        };

        public LightingState State { get; set; } = DefaultState;
        public double BrightnessPercent { get; set; } = DefaultBrightnessPercent;
        public double Temperature { get; set; } = DefaultTemperature;
        public LightingColorEntity Color { get; set; } = LightingColorEntity.Default.Copy();

        public static readonly Expression<Func<LightingEntity, LightingModel>> ToModelExpression = 
            l => new LightingModel(l.State, l.BrightnessPercent, l.Temperature, new LightingColorModel(l.Color.Red, l.Color.Green, l.Color.Blue));

        private static readonly Lazy<Func<LightingEntity, LightingModel>> ToModelFunc = new(ToModelExpression.Compile);

        public LightingModel ToModel()
        {
            return ToModelFunc.Value(this);
        }
        
        public LightingEntity Copy()
        {
            return new()
            {
                BrightnessPercent = BrightnessPercent,
                Color = Color?.Copy(),
                State = State,
                Temperature = Temperature
            };
        }

        protected bool Equals(LightingEntity other)
        {
            return State == other.State 
                   && Nullable.Equals(BrightnessPercent, other.BrightnessPercent) 
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
            return HashCode.Combine(State, BrightnessPercent, Temperature, Color);
        }

        public static LightingEntity FromModel(LightingModel model)
        {
            return new LightingEntity
            {
                State = model.State,
                BrightnessPercent = model.BrightnessPercent,
                Color = LightingColorEntity.FromModel(model.Color),
                Temperature = model.Temperature
            };
        }

        public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, LightingEntity> builder) 
            where TEntity : class
        {
            builder.Property(l => l.State).HasConversion<string>();
            builder.OwnsOne(l => l.Color);
        }
    }
}