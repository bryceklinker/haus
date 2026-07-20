using System;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Cqrs.Events;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default)
        where TEvent : IEvent;
}

internal class EventBus(IServiceProvider services) : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default)
        where TEvent : IEvent
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        return HandlerInvoker.InvokeAllAsync(services, handlerType, @event, token);
    }
}
