using System;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Site.Host.Shared.Realtime;

public interface IRealtimeDataSubscriber : IAsyncDisposable
{
    RealtimeDataState State { get; }
    void Subscribe<T>(string eventName, Func<T, Task> handler);
    Task StartAsync(CancellationToken cancellationToken = default);
}
