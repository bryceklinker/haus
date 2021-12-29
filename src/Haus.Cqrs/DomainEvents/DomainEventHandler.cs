using MediatR;

namespace Haus.Cqrs.DomainEvents;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}