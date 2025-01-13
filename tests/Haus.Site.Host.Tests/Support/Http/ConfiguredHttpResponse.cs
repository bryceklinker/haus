using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Http;

public record ConfiguredHttpResponse
{
    private readonly HttpRequestMessage _request;
    private readonly HttpResponseMessage _response;
    private readonly ConfigureHttpResponseOptions _options;

    public HttpMethod Method => _request.Method;
    public Uri? Uri => _request.RequestUri;
    
    public ConfiguredHttpResponse(
        HttpRequestMessage request,
        HttpResponseMessage response,
        ConfigureHttpResponseOptions options)
    {
        _request = request;
        _response = response;
        _options = options;
    }

    public async Task<HttpResponseMessage?> GetResponseAsync(HttpRequestMessage incomingRequest)
    {
        if (incomingRequest.Method != Method)
            return null;

        if (!DoUrisMatch(incomingRequest.RequestUri))
            return null;

        await Task.Delay(_options.Delay);
        return _response;
    }

    private bool DoUrisMatch(Uri? incomingUri)
    {
        if (incomingUri == null || Uri == null)
            return false;

        if (Uri == incomingUri)
            return true;

        return Uri.IsBaseOf(incomingUri);
    }
}