using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.Events
{
    public interface IEventBus
    {
        Task Publish<TEvent>(TEvent @event, CancellationToken token = default)
            where TEvent : IEvent;
    }
    public class EventBus : IEventBus
    {
        private readonly IMediator _mediator;

        public EventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish<TEvent>(TEvent @event, CancellationToken token = default) 
            where TEvent : IEvent
        {
            await _mediator.Publish(@event, token).ConfigureAwait(false);
        }
    }
}