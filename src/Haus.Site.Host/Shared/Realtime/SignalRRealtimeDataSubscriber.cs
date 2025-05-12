using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Haus.Site.Host.Shared.Realtime;

public class SignalRRealtimeDataSubscriber(HubConnection hub) : IRealtimeDataSubscriber
{
    private readonly ConcurrentBag<IDisposable> _subscriptions = [];

    public RealtimeDataState State => hub.State.ToRealtimeDataState();

    public void Subscribe<T>(string eventName, Func<T, Task> handler)
    {
        var subscription = hub.On(eventName, handler);
        _subscriptions.Add(subscription);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await hub.StartAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var subscription in _subscriptions)
            subscription.Dispose();
        await hub.DisposeAsync();
    }
}
