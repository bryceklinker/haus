using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;

namespace Haus.Site.Host.Tests.Support.Http;

public class InMemoryHttpMessageHandler : HttpMessageHandler
{
    private readonly ConcurrentBag<ConfiguredHttpResponse> _responses = [];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var incomingRequest = await request.CloneAsync();
        foreach (var response in _responses)
        {
            var result = await response.GetResponseAsync(incomingRequest);
            if (result == null)
                continue;
            return await result.CloneAsync();
        }

        return new HttpResponseMessage(HttpStatusCode.NotFound);
    }

    public async Task SetupResponse(
        HttpRequestMessage request,
        HttpResponseMessage response,
        ConfigureHttpResponseWithStatus options)
    {
        var requestClone = await request.CloneAsync();
        var responseClone = await response.CloneAsync();
        _responses.Add(new ConfiguredHttpResponse(requestClone, responseClone, options));
    }
    
    public async Task SetupResponse(
        HttpRequestMessage request,
        HttpResponseMessage response,
        Func<ConfigureHttpResponseWithStatus, ConfigureHttpResponseWithStatus>? configure = null)
    {
        var options = ConfigureOptions(configure);
        await SetupResponse(request, response, options);
    }

    public async Task SetupResponseAsJson<T>(
        HttpMethod method,
        string url,
        T model,
        Func<ConfigureHttpResponseWithStatus, ConfigureHttpResponseWithStatus>? configure = null)
    {
        var opts = ConfigureOptions(configure);

        var uri = CreateUriFromString(url, opts.BaseUri);
        var request = new HttpRequestMessage(method, uri);
        var response = new HttpResponseMessage(opts.Status)
        {
            Content = JsonContent.Create(model, options: HausJsonSerializer.DefaultOptions)
        };
        await SetupResponse(request, response, configure);
    }

    public async Task SetupGetAsJson<T>(string url, T model, Func<ConfigureHttpResponseWithStatus, ConfigureHttpResponseWithStatus>? configure = null)
    {
        await SetupResponseAsJson(HttpMethod.Get, url, model, configure);
    }

    public async Task SetupPostAsJson<T>(string url, T model, Func<ConfigureHttpResponseWithStatus, ConfigureHttpResponseWithStatus>? configure = null)
    {
        await SetupResponseAsJson(HttpMethod.Post, url, model, configure);
    }

    public async Task SetupPutAsJson<T>(string url, T model, Func<ConfigureHttpResponseWithStatus, ConfigureHttpResponseWithStatus>? configure = null)
    {
        await SetupResponseAsJson(HttpMethod.Put, url, model, configure);
    }

    public async Task SetupDeleteAsJson<T>(string url, Func<ConfigureHttpResponseWithStatus, ConfigureHttpResponseWithStatus>? configure = null)
    {
        await SetupResponseAsJson<object?>(HttpMethod.Delete, url, null, configure);
    }

    private static Uri CreateUriFromString(string url, Uri baseUri)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
            return uri;
        
        return new Uri(baseUri, url);
    }

    private static ConfigureHttpResponseWithStatus ConfigureOptions(
        Func<ConfigureHttpResponseWithStatus, ConfigureHttpResponseWithStatus>? configure = null)
    {
        var defaultOptions = new ConfigureHttpResponseWithStatus(Delay: TimeSpan.Zero);
        return configure != null
            ? configure.Invoke(defaultOptions)
            : defaultOptions;
    }
}