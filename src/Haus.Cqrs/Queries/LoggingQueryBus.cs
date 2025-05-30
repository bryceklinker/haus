using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs.Queries;

internal class LoggingQueryBus(IQueryBus queryBus, ILogger<LoggingQueryBus> logger) : LoggingBus(logger), IQueryBus
{
    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
    {
        return await ExecuteWithLoggingAsync(query, () => queryBus.ExecuteAsync(query, token), token)
            .ConfigureAwait(false);
    }

    protected override void LogFinished<TInput>(TInput input, long elapsedMilliseconds)
    {
        Logger.LogInformation("Finished executing query {Query} in {ElapsedTime}ms", input, elapsedMilliseconds);
    }

    protected override void LogError<TInput>(TInput input, Exception exception, long elapsedMilliseconds)
    {
        Logger.LogError(
            "Query {Query} failed to execute after {ElapsedTime}ms: {Exception}",
            input,
            exception,
            elapsedMilliseconds
        );
    }

    protected override void LogStarted<TInput>(TInput input)
    {
        Logger.LogInformation("Starting to execute query {Query}", input);
    }
}
