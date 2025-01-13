using System.Collections.Generic;
using System.Net.Http;

namespace Haus.Site.Host.Tests.Support.Http;

public class InMemoryHttpClientFactory : IHttpClientFactory
{
    private readonly Dictionary<string, InMemoryHttpMessageHandler> _handlers = new();

    public InMemoryHttpMessageHandler GetHandler(string name = "")
    {
        if (_handlers.TryGetValue(name, out var handler))
            return handler;
        
        _handlers.TryAdd(name, new InMemoryHttpMessageHandler());
        return _handlers[name];
    }
    
    public HttpClient CreateClient(string name)
    {
        var handler = GetHandler(name);
        return new HttpClient(handler);
    }
}