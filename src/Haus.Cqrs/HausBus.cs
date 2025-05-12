using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;

namespace Haus.Cqrs;

public interface IHausBus : IDomainEventBus
{
    Task ExecuteCommandAsync(ICommand command, CancellationToken token = default);
    Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default);
    Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default);
    Task PublishAsync(IEvent @event, CancellationToken token = default);
}

internal class HausBus(ICommandBus commandBus, IQueryBus queryBus, IEventBus eventBus, IDomainEventBus domainEventBus)
    : IHausBus
{
    public async Task ExecuteCommandAsync(ICommand command, CancellationToken token = default)
    {
        await commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteCommandAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken token = default
    )
    {
        return await commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
    {
        return await queryBus.ExecuteAsync(query, token).ConfigureAwait(false);
    }

    public async Task PublishAsync(IEvent @event, CancellationToken token = default)
    {
        await eventBus.PublishAsync(@event, token).ConfigureAwait(false);
    }

    public void Enqueue(IDomainEvent domainEvent)
    {
        domainEventBus.Enqueue(domainEvent);
    }

    public async Task FlushAsync(CancellationToken token = default)
    {
        await domainEventBus.FlushAsync(token).ConfigureAwait(false);
    }
}
