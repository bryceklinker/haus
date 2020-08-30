using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Haus.ServiceBus.Publish;

namespace Haus.Testing.Utilities.ServiceBus
{
    public class FakeHausServiceBusPublisher : IHausServiceBusPublisher
    {
        private readonly List<object> _publishedMessages = new List<object>();

        public Task PublishAsync<TPayload>(TPayload payload)
        {
            _publishedMessages.Add(payload);
            return Task.CompletedTask;
        }

        public IEnumerable<TPayload> GetMessages<TPayload>()
        {
            return _publishedMessages.OfType<TPayload>();
        }
    }
}