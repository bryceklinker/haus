using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting
{
    public class LightingColorEntity
    {
        private const byte DefaultColorValue = 255;
        public static readonly LightingColorEntity Default = new();

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public LightingColorEntity()
            : this(DefaultColorValue)
        {
        }

        public LightingColorEntity(
            byte red = DefaultColorValue, 
            byte green = DefaultColorValue,
            byte blue = DefaultColorValue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        protected bool Equals(LightingColorEntity other)
        {
            return Red == other.Red
                   && Green == other.Green
                   && Blue == other.Blue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LightingColorEntity) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Red, Green, Blue);
        }

        public static LightingColorEntity FromModel(LightingColorModel modelColor)
        {
            return new(
                modelColor?.Red ?? DefaultColorValue,
                modelColor?.Green ?? DefaultColorValue,
                modelColor?.Blue ?? DefaultColorValue
            );
        }

        public LightingColorEntity Copy()
        {
            return new(Red, Green, Blue);
        }
    }
}