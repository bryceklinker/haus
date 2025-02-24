using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public class InMemoryHttpResponse(
    HttpStatusCode statusCode,
    HttpResponseHeaders responseHeaders,
    byte[] content,
    HttpContentHeaders contentHeaders
)
{
    public HttpResponseMessage AsResponse()
    {
        var response = new HttpResponseMessage(statusCode);
        responseHeaders.CloneTo(response.Headers);

        response.Content = new ByteArrayContent(content);
        contentHeaders.CloneTo(response.Content.Headers);
        return response;
    }

    public static async Task<InMemoryHttpResponse> FromResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsByteArrayAsync();
        return new InMemoryHttpResponse(response.StatusCode, response.Headers, content, response.Content.Headers);
    }
}