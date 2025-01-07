using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.DomainEvents;

public interface IDomainEventBus
{
    void Enqueue(IDomainEvent domainEvent);
    Task FlushAsync(CancellationToken token = default);
}

internal class DomainEventBus(IMediator mediator) : IDomainEventBus
{
    private readonly ConcurrentQueue<IDomainEvent> _events = new();

    public void Enqueue(IDomainEvent domainEvent)
    {
        _events.Enqueue(domainEvent);
    }

    public async Task FlushAsync(CancellationToken token = default)
    {
        foreach (var domainEvent in _events)
            await mediator.Publish(domainEvent, token);
    }
}