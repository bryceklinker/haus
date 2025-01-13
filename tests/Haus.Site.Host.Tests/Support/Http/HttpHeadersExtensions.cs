using System.Net.Http.Headers;

namespace Haus.Site.Host.Tests.Support.Http;

public static class HttpHeadersExtensions
{
    public static void CloneTo(this HttpHeaders source, HttpHeaders target)
    {
        foreach (var header in source)
        {
            target.TryAddWithoutValidation(header.Key, header.Value);
        }
        
    }
}