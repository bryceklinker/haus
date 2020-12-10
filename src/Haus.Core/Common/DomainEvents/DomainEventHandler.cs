using MediatR;

namespace Haus.Core.Common.DomainEvents
{
    public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
        
    }
}