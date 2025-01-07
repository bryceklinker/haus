using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs.Events;

internal class LoggingEventBus(IEventBus eventBus, ILogger<LoggingEventBus> logger) : LoggingBus(logger), IEventBus
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
    {
        await ExecuteWithLoggingAsync(@event, async () =>
            {
                await eventBus.PublishAsync(@event, token).ConfigureAwait(false);
                return Unit.Value;
            }, token)
            .ConfigureAwait(false);
    }

    protected override void LogFinished<TInput>(TInput input, long elapsedMilliseconds)
    {
        Logger.LogInformation("Finished publishing {@Event} in {@ElapsedTime}ms", input, elapsedMilliseconds);
    }

    protected override void LogError<TInput>(TInput input, Exception exception, long elapsedMilliseconds)
    {
        Logger.LogError(exception, "Event {@Event} failed to publish after {@ElapsedTime}ms", input,
            elapsedMilliseconds);
    }

    protected override void LogStarted<TInput>(TInput input)
    {
        Logger.LogInformation("Starting to publish event {@Event}", input);
    }
}