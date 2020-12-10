using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Commands;
using Haus.Core.Common.DomainEvents;
using Haus.Core.Common.Events;
using Haus.Core.Common.Queries;

namespace Haus.Core.Common
{
    public interface IHausBus : IDomainEventBus
    {
        Task ExecuteCommandAsync(ICommand command, CancellationToken token = default);
        Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default);
        Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default);
        Task PublishAsync(IEvent @event, CancellationToken token = default);
    }
    
    public class HausBus : IHausBus
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IEventBus _eventBus;
        private readonly IDomainEventBus _domainEventBus;

        public HausBus(ICommandBus commandBus, IQueryBus queryBus, IEventBus eventBus, IDomainEventBus domainEventBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _eventBus = eventBus;
            _domainEventBus = domainEventBus;
        }

        public async Task ExecuteCommandAsync(ICommand command, CancellationToken token = default)
        {
            await _commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
        }

        public async Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            return await _commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            return await _queryBus.ExecuteAsync(query, token).ConfigureAwait(false);
        }

        public async Task PublishAsync(IEvent @event, CancellationToken token = default)
        {
            await _eventBus.PublishAsync(@event, token).ConfigureAwait(false);
        }

        public void Enqueue(IDomainEvent domainEvent)
        {
            _domainEventBus.Enqueue(domainEvent);
        }

        public async Task FlushAsync(CancellationToken token = default)
        {
            await _domainEventBus.FlushAsync(token).ConfigureAwait(false);
        }
    }
}