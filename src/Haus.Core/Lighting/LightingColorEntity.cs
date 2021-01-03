using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting
{
    public class LightingColorEntity
    {
        private const byte DefaultColorValue = 255;
        public static readonly LightingColorEntity Default = new LightingColorEntity
        {
            Blue = DefaultColorValue,
            Green = DefaultColorValue,
            Red = DefaultColorValue
        };
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

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
            return new()
            {
                Blue = modelColor?.Blue ?? DefaultColorValue,
                Green = modelColor?.Green ?? DefaultColorValue,
                Red = modelColor?.Red ?? DefaultColorValue
            };
        }

        public LightingColorEntity Copy()
        {
            return new()
            {
                Blue = Blue,
                Green = Green,
                Red = Red
            };
        }
    }
}