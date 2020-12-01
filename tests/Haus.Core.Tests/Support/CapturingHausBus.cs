using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Common.Queries;

namespace Haus.Core.Tests.Support
{
    public class CapturingHausBus : IHausBus
    {
        private readonly IHausBus _actualBus;
        private List<object> _messages;

        public CapturingHausBus(IHausBus actualBus)
        {
            _messages = new List<object>();
            _actualBus = actualBus;
        }

        public Task ExecuteCommandAsync(ICommand command, CancellationToken token = default)
        {
            _messages.Add(command);
            return _actualBus.ExecuteCommandAsync(command, token);
        }

        public Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            _messages.Add(command);
            return _actualBus.ExecuteCommandAsync(command, token);
        }

        public Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            _messages.Add(query);
            return _actualBus.ExecuteQueryAsync(query, token);
        }

        public Task PublishAsync(IEvent @event, CancellationToken token = default)
        {
            _messages.Add(@event);
            return _actualBus.PublishAsync(@event, token);
        }

        public IEnumerable<T> GetExecutedCommands<T>()
            where T : ICommand
        {
            return _messages.OfType<T>();
        }

        public IEnumerable<TCommand> GetExecutedCommands<TCommand, TResult>()
            where TCommand : ICommand<TResult>
        {
            return _messages.OfType<TCommand>();
        }

        public IEnumerable<TQuery> GetExecutedQueries<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            return _messages.OfType<TQuery>();
        }

        public IEnumerable<TEvent> GetPublishedEvents<TEvent>()
            where TEvent : IEvent
        {
            return _messages.OfType<TEvent>();
        }
    }
}