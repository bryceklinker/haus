using System;
using System.Net.Http;

namespace Haus.Web.Host.Tests.Support;

public class FakeHttpClientFactory(Func<HttpClient> creator) : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return creator();
    }
}