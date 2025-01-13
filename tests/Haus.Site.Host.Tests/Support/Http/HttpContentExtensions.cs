using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public static class HttpContentExtensions
{
    public static async Task<HttpContent> CloneAsync(this HttpContent content)
    {
        var memoryStream = new MemoryStream();
        var stream = await content.ReadAsStreamAsync();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        var clone = new StreamContent(memoryStream);
        content.Headers.CloneTo(clone.Headers);
        return clone;
    }
}