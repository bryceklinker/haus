using System;
using System.Net.Http;
using Haus.Api.Client.Options;
using Microsoft.Extensions.Options;

namespace Haus.Api.Client
{
    public interface IHausApiClientFactory
    {
        IHausApiClient Create();
    }

    public class HausApiClientFactory : IHausApiClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<HausApiClientSettings> _options;

        public HausApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<HausApiClientSettings> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public IHausApiClient Create()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_options.Value.BaseUrl);
            return new HausApiClient(httpClient);
        }
    }
}