using System;
using Haus.Core.Models.Common;

namespace Haus.Core.Common
{
    public class LightingColor
    {
        private const byte DefaultColorValue = 255;
        public static readonly LightingColor Default = new LightingColor
        {
            Blue = DefaultColorValue,
            Green = DefaultColorValue,
            Red = DefaultColorValue
        };
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        protected bool Equals(LightingColor other)
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
            return Equals((LightingColor) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Red, Green, Blue);
        }

        public static LightingColor FromModel(LightingColorModel modelColor)
        {
            return new LightingColor
            {
                Blue = modelColor?.Blue ?? DefaultColorValue,
                Green = modelColor?.Green ?? DefaultColorValue,
                Red = modelColor?.Red ?? DefaultColorValue
            };
        }

        public LightingColor Copy()
        {
            return new LightingColor
            {
                Blue = Blue,
                Green = Green,
                Red = Red
            };
        }
    }
}