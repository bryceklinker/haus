using System.Collections.Concurrent;
using System.Threading.Tasks;
using Haus.Site.Host.Shared.Realtime;

namespace Haus.Site.Host.Tests.Support.Realtime;

public class InMemoryRealtimeDataFactory : IRealtimeDataFactory
{
    private readonly ConcurrentDictionary<string, InMemoryRealtimeDataSubscriber> _subscribers = new();

    public Task<IRealtimeDataSubscriber> CreateSubscriber(string source)
    {
        return Task.FromResult<IRealtimeDataSubscriber>(GetSubscriber(source));
    }

    public InMemoryRealtimeDataSubscriber GetSubscriber(string source)
    {
        return _subscribers.GetOrAdd(source, _ => new InMemoryRealtimeDataSubscriber());
    }
}
