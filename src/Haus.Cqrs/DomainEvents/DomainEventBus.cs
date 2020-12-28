using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.DomainEvents
{
    public interface IDomainEventBus
    {
        void Enqueue(IDomainEvent domainEvent);
        Task FlushAsync(CancellationToken token = default);
    }

    internal class DomainEventBus : IDomainEventBus
    {
        private readonly ConcurrentQueue<IDomainEvent> _events;
        private readonly IMediator _mediator;

        public DomainEventBus(IMediator mediator)
        {
            _mediator = mediator;
            _events = new ConcurrentQueue<IDomainEvent>();
        }

        public void Enqueue(IDomainEvent domainEvent)
        {
            _events.Enqueue(domainEvent);
        }

        public async Task FlushAsync(CancellationToken token = default)
        {
            foreach (var domainEvent in _events) 
                await _mediator.Publish(domainEvent, token);
        }
    }
}