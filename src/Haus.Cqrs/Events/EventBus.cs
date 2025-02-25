using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.Events;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default)
        where TEvent : IEvent;
}

internal class EventBus(IMediator mediator) : IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default)
        where TEvent : IEvent
    {
        await mediator.Publish(@event, token).ConfigureAwait(false);
    }
}
