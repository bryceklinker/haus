using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
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
        ConfigureHttpResponseOptions? options = null)
    {
        var requestClone = await request.CloneAsync();
        var responseClone = await response.CloneAsync();
        var configuredOptions = options ?? new ConfigureHttpResponseOptions(TimeSpan.Zero);
        _responses.Add(new ConfiguredHttpResponse(requestClone, responseClone, configuredOptions));
    }

    public async Task SetupResponseAsJson<T>(
        HttpMethod method,
        string url,
        T model,
        ConfigureHttpResponseWithStatus? options = null)
    {
        var opts = options ?? new ConfigureHttpResponseWithStatus();

        var uri = CreateUriFromString(url, opts.BaseUri);
        var request = new HttpRequestMessage(method, uri);
        var response = new HttpResponseMessage(opts.Status)
        {
            Content = JsonContent.Create(model, options: HausJsonSerializer.DefaultOptions)
        };
        await SetupResponse(request, response, opts);
    }

    public async Task SetupGetAsJson<T>(string url, T model, ConfigureHttpResponseWithStatus? options = null)
    {
        await SetupResponseAsJson(HttpMethod.Get, url, model, options);
    }

    public async Task SetupPostAsJson<T>(string url, T model, ConfigureHttpResponseWithStatus? options = null)
    {
        await SetupResponseAsJson(HttpMethod.Post, url, model, options);
    }

    public async Task SetupPutAsJson<T>(string url, T model, ConfigureHttpResponseWithStatus? options = null)
    {
        await SetupResponseAsJson(HttpMethod.Put, url, model, options);
    }

    public async Task SetupDeleteAsJson<T>(string url, ConfigureHttpResponseWithStatus? options = null)
    {
        await SetupResponseAsJson<object?>(HttpMethod.Delete, url, null, options);
    }

    private static Uri CreateUriFromString(string url, Uri baseUri)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
            return uri;
        
        return new Uri(baseUri, url);
    }
}