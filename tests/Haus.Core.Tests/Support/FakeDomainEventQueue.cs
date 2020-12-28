using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.DomainEvents;

namespace Haus.Core.Tests.Support
{
    public class FakeDomainEventBus : IDomainEventBus
    {
        private readonly Queue<IDomainEvent> _queue = new Queue<IDomainEvent>();

        public IEnumerable<IDomainEvent> GetEvents => _queue.AsEnumerable();
        
        public void Enqueue(IDomainEvent domainEvent)
        {
            _queue.Enqueue(domainEvent);
        }

        public Task FlushAsync(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
}