using MediatR;

namespace Haus.Cqrs.Events
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IEvent
    {
        
    }
}