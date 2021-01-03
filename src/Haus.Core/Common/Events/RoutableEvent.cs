using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Cqrs.Events;

namespace Haus.Core.Common.Events
{
    public record RoutableEvent : IEvent
    {
        public static RoutableEvent<T> FromEvent<T>(T @event)
            where T : IHausEventCreator<T>
        {
            return new(@event.AsHausEvent());
        }
    }

    public record RoutableEvent<T> : RoutableEvent
    {
        private readonly HausEvent<T> _hausEvent;
        
        public string Type => _hausEvent.Type;
        public T Payload => _hausEvent.Payload;

        public RoutableEvent(HausEvent<T> hausEvent)
        {
            _hausEvent = hausEvent;
        }
    }
}