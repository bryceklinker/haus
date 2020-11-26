using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Common
{
    internal abstract class LoggingBus
    {
        private readonly ILogger _logger;

        protected abstract string BeginMessage { get; }

        protected abstract string FinishedMessage { get; }

        protected abstract string ErrorMessage { get; }

        protected LoggingBus(ILogger logger)
        {
            _logger = logger;
        }
        
        protected async Task<TOutput> ExecuteWithLoggingAsync<TInput, TOutput>(TInput input, Func<Task<TOutput>> executor, CancellationToken token = default)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var output = await executor.Invoke().ConfigureAwait(false);
                stopwatch.Stop();
                _logger.LogInformation(FinishedMessage, new {input, stopwatch.ElapsedMilliseconds});
                return output;
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                _logger.LogError(ErrorMessage, new {input, e, stopwatch.ElapsedMilliseconds});
                throw;
            }
        }
    }
}