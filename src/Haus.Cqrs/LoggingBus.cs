using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs
{
    internal abstract class LoggingBus
    {
        protected ILogger Logger { get; }

        protected LoggingBus(ILogger logger)
        {
            Logger = logger;
        }
        
        protected async Task<TOutput> ExecuteWithLoggingAsync<TInput, TOutput>(TInput input, Func<Task<TOutput>> executor, CancellationToken token = default)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                LogStarted(input);
                var output = await executor.Invoke().ConfigureAwait(false);
                stopwatch.Stop();
                LogFinished(input, stopwatch.ElapsedMilliseconds);
                return output;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                LogError(input, e, stopwatch.ElapsedMilliseconds);
                LogError(input, e, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
        
        protected abstract void LogFinished<TInput>(TInput input, long elapsedMilliseconds);
        protected abstract void LogError<TInput>(TInput input, Exception exception, long elapsedMilliseconds);
        protected abstract void LogStarted<TInput>(TInput input);
    }
}