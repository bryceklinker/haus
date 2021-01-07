using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Lighting
{
    public class LightingColorEntity
    {
        public static readonly LightingColorEntity Default = new();

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public LightingColorEntity()
            : this(LightingDefaults.Red)
        {
        }

        public LightingColorEntity(
            byte red = LightingDefaults.Red, 
            byte green = LightingDefaults.Green,
            byte blue = LightingDefaults.Blue)
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
                modelColor?.Red ?? LightingDefaults.Red,
                modelColor?.Green ?? LightingDefaults.Green,
                modelColor?.Blue ?? LightingDefaults.Blue
            );
        }

        public LightingColorEntity Copy()
        {
            return new(Red, Green, Blue);
        }
    }
}