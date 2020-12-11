using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client.Options;
using Haus.Core.Models;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client.Common
{
    public interface IApiClient
    {
        string BaseUrl { get; }    
    }
    
    public abstract class ApiClient : IApiClient
    {
        private const string ApiPath = "api";
        private readonly HttpClient _httpClient;
        private readonly IOptions<HausApiClientSettings> _options;

        public string BaseUrl => UrlUtility.Join(null, _options.Value.BaseUrl, ApiPath);
        
        protected ApiClient(HttpClient httpClient, IOptions<HausApiClientSettings> options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        protected Task<T> GetAsJsonAsync<T>(string path, QueryParameters parameters = null)
        {
            var fullUrl = GetFullUrl(path, parameters);
            return _httpClient.GetFromJsonAsync<T>(fullUrl);
        }

        protected Task<HttpResponseMessage> PostAsJsonAsync<T>(string path, T data, QueryParameters parameters = null)
        {
            var fullUrl = GetFullUrl(path, parameters);
            return _httpClient.PostAsJsonAsync(fullUrl, data);
        }

        protected Task<HttpResponseMessage> PostEmptyContentAsync(string path, QueryParameters parameters = null)
        {
            var fullUrl = GetFullUrl(path, parameters);
            return _httpClient.PostAsync(fullUrl, new ByteArrayContent(Array.Empty<byte>()));
        }

        protected Task<HttpResponseMessage> PutAsJsonAsync<T>(string path, T data, QueryParameters parameters = null)
        {
            var fullUrl = GetFullUrl(path, parameters);
            return _httpClient.PutAsJsonAsync(fullUrl, data);
        }

        private string GetFullUrl(string path, QueryParameters parameters)
        {
            return UrlUtility.Join(parameters, BaseUrl, path);
        }
    }
}