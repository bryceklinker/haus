using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Site.Host.Shared.Realtime;

namespace Haus.Site.Host.Tests.Support.Realtime;

public class InMemoryRealtimeDataSubscriber : IRealtimeDataSubscriber
{
    private readonly List<InMemoryRealtimeDataHandler> _handlers = new();
    private TimeSpan _startDelay = TimeSpan.Zero;

    public bool IsStarted { get; private set; }
    public bool IsDisposed { get; private set; }

    public RealtimeDataState State { get; private set; } = RealtimeDataState.Disconnected;

    private InMemoryRealtimeDataHandler[] Handlers
    {
        get
        {
            lock (_handlers)
            {
                return _handlers.ToArray();
            }
        }
    }

    public void Subscribe<T>(string eventName, Func<T, Task> handler)
    {
        lock (_handlers)
        {
            _handlers.Add(InMemoryRealtimeDataHandler.Create(eventName, handler));
        }
    }

    public async Task SimulateAsync<T>(string eventName, T data)
    {
        foreach (var handler in Handlers)
            await handler.InvokeAsync(eventName, data);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        IsStarted = true;
        await Task.Delay(_startDelay, cancellationToken);
        State = RealtimeDataState.Connected;
    }

    public ValueTask DisposeAsync()
    {
        IsStarted = false;
        IsDisposed = true;
        return ValueTask.CompletedTask;
    }

    public void ConfigureStartDelay(TimeSpan delay) => _startDelay = delay;

    public void ConfigureStartDelayMs(int ms) => ConfigureStartDelay(TimeSpan.FromMilliseconds(ms));
}
