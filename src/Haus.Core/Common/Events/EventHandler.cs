using MediatR;

namespace Haus.Core.Common.Events
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IEvent
    {

    }
}