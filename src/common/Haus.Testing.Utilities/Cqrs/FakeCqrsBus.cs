using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;

namespace Haus.Testing.Utilities.Cqrs
{
    public class FakeCqrsBus : ICqrsBus
    {
        private readonly List<object> _executedCommands = new List<object>();
        private readonly List<IEvent> _publishedEvents = new List<IEvent>();
        private readonly Dictionary<object, object> _commandResults = new Dictionary<object, object>();
        private readonly Dictionary<object, object> _queryResults = new Dictionary<object, object>();
        
        public Task ExecuteCommand(ICommand command, CancellationToken token = default)
        {
            _executedCommands.Add(command);
            return Task.CompletedTask;
        }

        public Task<TResult> ExecuteCommand<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            _executedCommands.Add(command);
            if (_commandResults.ContainsKey(command))
                return Task.FromResult((TResult) _commandResults[command]);
            
            throw new InvalidOperationException($"No result setup for command {command.GetType()}.");
        }

        public Task<TResult> ExecuteQuery<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            if (_queryResults.ContainsKey(query))
                return Task.FromResult((TResult) _queryResults[query]);
            
            throw new InvalidOperationException($"No result setup for query {query.GetType()}.");
        }

        public Task Publish<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
        {
            _publishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public void SetupCommandResult<TResult>(ICommand<TResult> command, TResult result)
        {
            _commandResults.Add(command, result);
        }

        public void SetupQueryResult<TResult>(IQuery<TResult> query, TResult result)
        {
            _queryResults.Add(query, result);
        }

        public IEnumerable<TCommand> GetExecutedCommand<TCommand>()
            where TCommand : ICommand
        {
            return _executedCommands.OfType<TCommand>();
        }
        
        public IEnumerable<TCommand> GetExecutedCommand<TCommand, TResult>()
            where TCommand : ICommand<TResult>
        {
            return _executedCommands.OfType<TCommand>();
        }
    }
}