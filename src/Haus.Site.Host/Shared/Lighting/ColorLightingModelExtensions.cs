using System;
using Haus.Core.Models.Lighting;

namespace Haus.Site.Host.Shared.Lighting;

public static class ColorLightingModelExtensions
{
    public static string ToHex(this ColorLightingModel model)
    {
        var hex = Convert.ToHexStringLower([model.Red, model.Green, model.Blue]);
        return $"#{hex}";
    }

    public static string ToHex(this byte value)
    {
        return $"{value:X2}";
    }
}
