using System;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Cqrs.Commands;

public interface ICommandBus
{
    Task ExecuteAsync(ICommand command, CancellationToken token = default);
    Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default);
}

internal class CommandBus(IServiceProvider services) : ICommandBus
{
    public Task ExecuteAsync(ICommand command, CancellationToken token = default)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        return HandlerInvoker.InvokeAsync(services, handlerType, command, token);
    }

    public Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        return HandlerInvoker.InvokeAsync<TResult>(services, handlerType, command, token);
    }
}
