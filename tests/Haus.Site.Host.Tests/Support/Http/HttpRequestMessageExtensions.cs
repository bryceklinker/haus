using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public static class HttpRequestMessageExtensions
{
    public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage
        {
            Method = request.Method,
            RequestUri = request.RequestUri,
            Version = request.Version,
            VersionPolicy = request.VersionPolicy,
            Content = request.Content == null
                ? null
                : await request.Content.CloneAsync(),
        };
        request.Headers.CloneTo(clone.Headers);
        foreach (var option in request.Options)
            clone.Options.TryAdd(option.Key, option.Value);
        
        return clone;
    }
}