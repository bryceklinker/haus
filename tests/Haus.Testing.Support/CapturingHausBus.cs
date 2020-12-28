using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;

namespace Haus.Testing.Support
{
    public class CapturingHausBus : IHausBus
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IEventBus _eventBus;
        private readonly IDomainEventBus _domainEventBus;
        private readonly List<object> _messages;

        public CapturingHausBus(ICommandBus commandBus, IQueryBus queryBus, IEventBus eventBus, IDomainEventBus domainEventBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _eventBus = eventBus;
            _domainEventBus = domainEventBus;
            _messages = new List<object>();
        }

        public Task ExecuteCommandAsync(ICommand command, CancellationToken token = default)
        {
            _messages.Add(command);
            return _commandBus.ExecuteAsync(command, token);
        }

        public Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
        {
            _messages.Add(command);
            return _commandBus.ExecuteAsync(command, token);
        }

        public Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            _messages.Add(query);
            return _queryBus.ExecuteAsync(query, token);
        }

        public Task PublishAsync(IEvent @event, CancellationToken token = default)
        {
            _messages.Add(@event);
            return _eventBus.PublishAsync(@event, token);
        }

        public void Enqueue(IDomainEvent domainEvent)
        {
            _messages.Add(domainEvent);
            _domainEventBus.Enqueue(domainEvent);
        }

        public Task FlushAsync(CancellationToken token = default)
        {
            return _domainEventBus.FlushAsync(token);
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

        public IEnumerable<HausCommand<T>> GetPublishedHausCommands<T>()
        {
            return _messages.OfType<RoutableCommand>()
                .Select(r => r.HausCommand)
                .OfType<HausCommand<T>>();
        }

        public IEnumerable<TDomainEvent> GetQueuedDomainEvents<TDomainEvent>()
            where TDomainEvent : IDomainEvent
        {
            return _messages.OfType<TDomainEvent>();
        }
        
    }
}