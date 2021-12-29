using System.Linq;

namespace Haus.Api.Client.Common;

internal static class UrlUtility
{
    public static string Join(QueryParameters queryParameters = null, params string[] paths)
    {
        var trimmedPaths = paths.Select(TrimSlashes);
        var baseUrl = string.Join("/", trimmedPaths);
        return queryParameters == null
            ? baseUrl
            : $"{baseUrl}?{queryParameters}";
    }

    private static string TrimSlashes(string value)
    {
        var trimmed = value;
        if (trimmed.StartsWith("/")) trimmed = trimmed.Substring(1);
        if (trimmed.EndsWith("/")) trimmed = trimmed.Substring(0, trimmed.Length - 1);
        return trimmed;
    }
}