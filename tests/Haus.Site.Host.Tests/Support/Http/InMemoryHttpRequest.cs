using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public record InMemoryHttpRequest(
    HttpMethod Method,
    Uri? Uri,
    HttpRequestHeaders RequestHeaders,
    byte[] Content,
    HttpContentHeaders? ContentHeaders
)
{
    public HttpRequestMessage AsRequest()
    {
        var request = new HttpRequestMessage(Method, Uri);
        RequestHeaders.CloneTo(request.Headers);
        
        request.Content = new ByteArrayContent(Content);
        ContentHeaders?.CloneTo(request.Content.Headers);
        return request;
    }
    
    public static async Task<InMemoryHttpRequest> FromRequest(HttpRequestMessage request)
    {
        var content = request.Content == null
            ? []
            : await request.Content.ReadAsByteArrayAsync();
        return new InMemoryHttpRequest(request.Method, request.RequestUri, request.Headers, content, request.Content?.Headers);
    }
}