using Haus.Core.Models.Common;

namespace Haus.Core.Common
{
    public class LightingColor
    {
        public byte? Red { get; set; }
        public byte? Green { get; set; }
        public byte? Blue { get; set; }

        public static LightingColor FromModel(LightingColorModel modelColor)
        {
            return new LightingColor
            {
                Blue = modelColor?.Blue,
                Green = modelColor?.Green,
                Red = modelColor?.Red
            };
        }
    }
}