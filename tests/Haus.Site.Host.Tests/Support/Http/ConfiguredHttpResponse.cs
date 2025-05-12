using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public record ConfiguredHttpResponse(
    InMemoryHttpRequest Request,
    InMemoryHttpResponse Response,
    ConfigureHttpResponseOptions Options
)
{
    public async Task<HttpResponseMessage?> GetResponseAsync(HttpRequestMessage incomingRequest)
    {
        var request = await InMemoryHttpRequest.FromRequest(incomingRequest);
        if (request.Method != Request.Method)
            return null;

        if (!DoUrisMatch(incomingRequest.RequestUri))
            return null;

        await Task.Delay(Options.Delay);
        await Options.Capture(request.AsRequest());
        return Response.AsResponse();
    }

    private bool DoUrisMatch(Uri? incomingUri)
    {
        if (incomingUri == null || Request.Uri == null)
            return false;

        if (Request.Uri == incomingUri)
            return true;

        return Request.Uri.IsBaseOf(incomingUri) && Request.Uri.AbsolutePath == incomingUri.AbsolutePath;
    }
}
