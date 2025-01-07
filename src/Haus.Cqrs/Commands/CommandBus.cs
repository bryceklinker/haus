using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Haus.Cqrs.Commands;

public interface ICommandBus
{
    Task ExecuteAsync(ICommand command, CancellationToken token = default);
    Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default);
}

internal class CommandBus(IMediator mediator) : ICommandBus
{
    public async Task ExecuteAsync(ICommand command, CancellationToken token = default)
    {
        await mediator.Send(command, token).ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
    {
        return await mediator.Send(command, token).ConfigureAwait(false);
    }
}