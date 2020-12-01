using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Common.Events
{
    public class RoutableEvent : IEvent
    {
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