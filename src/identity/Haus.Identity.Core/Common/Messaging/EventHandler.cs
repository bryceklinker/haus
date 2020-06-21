using MediatR;

namespace Haus.Identity.Core.Common.Messaging
{
    public interface IEventHandler<TEvent> : INotificationHandler<TEvent>
        where TEvent : IEvent
    {
        
    }
}