using System.Text.Json;
using Haus.Core.Models;

namespace Haus.Site.Host.Shared.Formatting;

public static class FormatJsonExtensions
{
    public static string FormatAsJson(this object? value)
    {
        return HausJsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
    }
}
