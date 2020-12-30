using System;
using Haus.Core.Models.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Common
{
    public class Lighting
    {
        private const LightingState DefaultState = LightingState.Off;
        private const double DefaultTemperature = 150;
        private const double DefaultBrightnessPercent = 100;
        public static readonly Lighting Default = new()
        {
            State = DefaultState,
            BrightnessPercent = DefaultBrightnessPercent,
            Color = LightingColor.Default,
            Temperature = DefaultTemperature
        };

        public LightingState State { get; set; } = DefaultState;
        public double BrightnessPercent { get; set; } = DefaultBrightnessPercent;
        public double Temperature { get; set; } = DefaultTemperature;
        public LightingColor Color { get; set; } = LightingColor.Default.Copy();

        public Lighting Copy()
        {
            return new()
            {
                BrightnessPercent = BrightnessPercent,
                Color = Color?.Copy(),
                State = State,
                Temperature = Temperature
            };
        }

        protected bool Equals(Lighting other)
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
            return Equals((Lighting) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(State, BrightnessPercent, Temperature, Color);
        }

        public static Lighting FromModel(LightingModel model)
        {
            return new Lighting
            {
                State = model.State,
                BrightnessPercent = model.BrightnessPercent,
                Color = LightingColor.FromModel(model.Color),
                Temperature = model.Temperature
            };
        }

        public static void Configure<TEntity>(OwnedNavigationBuilder<TEntity, Lighting> builder) 
            where TEntity : class
        {
            builder.Property(l => l.State).HasConversion<string>();
            builder.OwnsOne(l => l.Color);
        }
    }
}