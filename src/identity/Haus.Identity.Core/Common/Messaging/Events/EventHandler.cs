using MediatR;

namespace Haus.Identity.Core.Common.Messaging.Events
{
    public interface IEventHandler<TEvent> : INotificationHandler<TEvent>
        where TEvent : IEvent
    {
        
    }
}