using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Cqrs.DomainEvents;

public interface IDomainEventBus
{
    void Enqueue(IDomainEvent domainEvent);
    Task FlushAsync(CancellationToken token = default);
}

internal class DomainEventBus(IServiceProvider services) : IDomainEventBus
{
    private readonly ConcurrentQueue<IDomainEvent> _events = new();

    public void Enqueue(IDomainEvent domainEvent)
    {
        _events.Enqueue(domainEvent);
    }

    public async Task FlushAsync(CancellationToken token = default)
    {
        foreach (var domainEvent in _events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            await HandlerInvoker.InvokeAllAsync(services, handlerType, domainEvent, token).ConfigureAwait(false);
        }
    }
}
