using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Haus.Web.Host.Tests.Support;

public class FakeHttpClientFactory(
    Func<HttpClient> defaultCreator,
    IReadOnlyDictionary<string, Func<HttpClient>>? byName = null
) : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        if (byName != null && byName.TryGetValue(name, out var creator))
            return creator();
        return defaultCreator();
    }
}
