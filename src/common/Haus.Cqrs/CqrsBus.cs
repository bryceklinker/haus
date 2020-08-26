using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;

namespace Haus.Cqrs
{
    public interface ICqrsBus
    {
        Task ExecuteCommand(ICommand command, CancellationToken token = default);
        Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken token = default);
        Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken token = default);
        Task Publish<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent;
    }
    
    public class CqrsBus : ICqrsBus
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IEventBus _eventBus;

        public CqrsBus(ICommandBus commandBus, IQueryBus queryBus, IEventBus eventBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _eventBus = eventBus;
        }

        public async Task ExecuteCommand(ICommand command, CancellationToken token = default)
        {
            await _commandBus.Execute(command, token).ConfigureAwait(false);
        }

        public async Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            return await _commandBus.Execute(command, token).ConfigureAwait(false);
        }

        public async Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            return await _queryBus.Execute(query, token).ConfigureAwait(false);
        }

        public async Task Publish<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
        {
            await _eventBus.Publish(@event, token).ConfigureAwait(false);
        }
    }
}