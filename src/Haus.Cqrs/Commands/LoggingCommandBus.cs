using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs.Commands;

internal class LoggingCommandBus(ICommandBus commandBus, ILogger<LoggingCommandBus> logger)
    : LoggingBus(logger),
        ICommandBus
{
    public async Task ExecuteAsync(ICommand command, CancellationToken token = default)
    {
        await ExecuteWithLoggingAsync(
            command,
            async () =>
            {
                await commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
                return Unit.Value;
            },
            token
        );
    }

    public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
    {
        return await ExecuteWithLoggingAsync(command, () => commandBus.ExecuteAsync(command, token), token);
    }

    protected override void LogFinished<TInput>(TInput input, long elapsedMilliseconds)
    {
        Logger.LogInformation("Finished executing command {@Command} in {@ElapsedTime}ms", input, elapsedMilliseconds);
    }

    protected override void LogError<TInput>(TInput input, Exception exception, long elapsedMilliseconds)
    {
        Logger.LogError(
            exception,
            "Command {@Command} failed to execute after {@ElapsedTime}ms",
            input,
            elapsedMilliseconds
        );
    }

    protected override void LogStarted<TInput>(TInput input)
    {
        Logger.LogInformation("Starting to execute command {@Command}", input);
    }
}
