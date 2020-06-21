using System.Threading;
using System.Threading.Tasks;

namespace Haus.Identity.Core.Common.Messaging
{
    public interface IMessageBus : ICommandBus, IQueryBus, IEventBus
    {
    }

    public class MessageBus : IMessageBus
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IEventBus _eventBus;

        public MessageBus(ICommandBus commandBus, IQueryBus queryBus, IEventBus eventBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _eventBus = eventBus;
        }

        public async Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            return await _queryBus.ExecuteQuery(query, token);
        }

        public async Task ExecuteCommand(ICommand command, CancellationToken token = default) 
        {
            await _commandBus.ExecuteCommand(command, token);
        }

        public async Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            return await _commandBus.ExecuteCommand(command, token);
        }

        public async Task Publish<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
        {
            await _eventBus.Publish(@event, token);
        }
    }
}