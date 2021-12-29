using System;
using System.Net.Http;

namespace Haus.Web.Host.Tests.Support;

public class FakeHttpClientFactory : IHttpClientFactory
{
    private readonly Func<HttpClient> _creator;

    public FakeHttpClientFactory(Func<HttpClient> creator)
    {
        _creator = creator;
    }

    public HttpClient CreateClient(string name)
    {
        return _creator();
    }
}