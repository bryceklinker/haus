using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Common.Events
{
    public class RoutableEvent : IEvent
    {
        public static RoutableEvent<T> FromEvent<T>(T @event)
            where T : IHausEventCreator<T>
        {
            return new RoutableEvent<T>(@event.AsHausEvent());
        }
    }

    public class RoutableEvent<T> : RoutableEvent
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