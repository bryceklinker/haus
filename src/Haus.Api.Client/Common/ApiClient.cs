using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client.Options;
using Haus.Core.Models;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Common;

public interface IApiClient
{
    string ApiBaseUrl { get; }
    string BaseUrl { get; }
}

public abstract class ApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options) : IApiClient
{
    private const string ApiPath = "api";

    public string BaseUrl => options.Value.BaseUrl;
    public string ApiBaseUrl => UrlUtility.Join(null, BaseUrl, ApiPath);
    protected HttpClient HttpClient { get; } = httpClient;

    protected async Task<T?> GetAsJsonAsync<T>(string path, QueryParameters? parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return await HttpClient.GetFromJsonAsync<T>(fullUrl, HausJsonSerializer.DefaultOptions);
    }

    protected Task<HttpResponseMessage> PostAsJsonAsync<T>(string path, T data, QueryParameters? parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return HttpClient.PostAsJsonAsync(fullUrl, data, HausJsonSerializer.DefaultOptions);
    }

    protected Task<HttpResponseMessage> PostEmptyContentAsync(string path, QueryParameters? parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return HttpClient.PostAsync(fullUrl, new ByteArrayContent([]));
    }

    protected Task<HttpResponseMessage> PutAsJsonAsync<T>(string path, T data, QueryParameters? parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return HttpClient.PutAsJsonAsync(fullUrl, data, HausJsonSerializer.DefaultOptions);
    }

    protected string GetFullUrl(string path, QueryParameters? parameters = null)
    {
        return UrlUtility.Join(parameters, ApiBaseUrl, path);
    }
}
