using System.Collections.Generic;
using System.Linq;
using Haus.ServiceBus.Common;
using Haus.ServiceBus.Publish;

namespace Haus.Testing.Utilities.ServiceBus
{
    public class FakeHausServiceBusPublisher : IHausServiceBusPublisher
    {
        private readonly List<object> _publishedMessages = new List<object>();
        
        public void Publish<TPayload>(ServiceBusMessage<TPayload> message)
        {
            _publishedMessages.Add(message);
        }

        public IEnumerable<ServiceBusMessage<TPayload>> GetMessages<TPayload>()
        {
            return _publishedMessages.OfType<ServiceBusMessage<TPayload>>();
        }
    }
}