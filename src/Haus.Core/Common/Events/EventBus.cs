using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Core.Common.Events
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default)
            where TEvent : IEvent;
    }
    
    public class EventBus : IEventBus
    {
        private readonly IMediator _mediator;

        public EventBus(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default) 
            where TEvent : IEvent
        {
            await _mediator.Publish(@event, token).ConfigureAwait(false);
        }
    }
}