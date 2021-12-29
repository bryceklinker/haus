using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client.Options;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Common;

public interface IApiClient
{
    string ApiBaseUrl { get; }
    string BaseUrl { get; }
}

public abstract class ApiClient : IApiClient
{
    private const string ApiPath = "api";
    private readonly IOptions<HausApiClientSettings> _options;

    public string BaseUrl => UrlUtility.Join(null, _options.Value.BaseUrl);
    public string ApiBaseUrl => UrlUtility.Join(null, BaseUrl, ApiPath);
    protected HttpClient HttpClient { get; }

    protected ApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
    {
        HttpClient = httpClient;
        _options = options;
    }

    protected Task<T> GetAsJsonAsync<T>(string path, QueryParameters parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return HttpClient.GetFromJsonAsync<T>(fullUrl);
    }

    protected Task<HttpResponseMessage> PostAsJsonAsync<T>(string path, T data, QueryParameters parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return HttpClient.PostAsJsonAsync(fullUrl, data);
    }

    protected Task<HttpResponseMessage> PostEmptyContentAsync(string path, QueryParameters parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return HttpClient.PostAsync(fullUrl, new ByteArrayContent(Array.Empty<byte>()));
    }

    protected Task<HttpResponseMessage> PutAsJsonAsync<T>(string path, T data, QueryParameters parameters = null)
    {
        var fullUrl = GetFullUrl(path, parameters);
        return HttpClient.PutAsJsonAsync(fullUrl, data);
    }

    protected string GetFullUrl(string path, QueryParameters parameters = null)
    {
        return UrlUtility.Join(parameters, ApiBaseUrl, path);
    }
}