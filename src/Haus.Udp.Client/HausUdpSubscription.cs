using System;
using System.Threading.Tasks;
using Haus.Core.Models;

namespace Haus.Udp.Client;

public interface IHausUdpSubscription
{
    Guid Id { get; }
    Task ExecuteAsync(byte[] bytes);
    Task UnsubscribeAsync();
}

internal class HausUdpSubscription<T>(Func<T, Task> handler, Func<IHausUdpSubscription, Task> unsubscribe)
    : IHausUdpSubscription
{
    public Guid Id { get; } = Guid.NewGuid();

    public Task ExecuteAsync(byte[] bytes)
    {
        return HausJsonSerializer.TryDeserialize(bytes, out T value)
            ? handler.Invoke(value)
            : Task.CompletedTask;
    }

    public Task UnsubscribeAsync()
    {
        return unsubscribe.Invoke(this);
    }
}