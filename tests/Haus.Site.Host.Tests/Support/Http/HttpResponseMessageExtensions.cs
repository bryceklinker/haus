using System.Net.Http;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public static class HttpResponseMessageExtensions
{
    public static async Task<HttpResponseMessage> CloneAsync(this HttpResponseMessage response)
    {
        var clone = new HttpResponseMessage
        {
            StatusCode = response.StatusCode,
            Version = response.Version,
            ReasonPhrase = response.ReasonPhrase,
            RequestMessage = response.RequestMessage == null
                ? null
                : await response.RequestMessage.CloneAsync(),
            Content = await response.Content.CloneAsync(),
        };
        response.Headers.CloneTo(clone.Headers);
        response.TrailingHeaders.CloneTo(clone.TrailingHeaders);
        return clone;
    }
}