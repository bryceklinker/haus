using System.Net.Http;
using System.Net.Http.Headers;

namespace Haus.Web.Host.Tests.Support
{
    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HausWebHostApplicationFactory _factory;

        public FakeHttpClientFactory(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        public HttpClient CreateClient(string name)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestingAuthenticationHandler.TestingScheme);
            return client;
        }
    }
}