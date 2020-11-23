using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Common.Queries
{
    public class LoggingQueryBus : IQueryBus
    {
        private readonly IQueryBus _queryBus;
        private readonly ILogger<LoggingQueryBus> _logger;

        public LoggingQueryBus(IQueryBus queryBus, ILogger<LoggingQueryBus> logger)
        {
            _queryBus = queryBus;
            _logger = logger;
        }

        public async Task<TResult> Execute<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            try
            {
                var stopwatch = new Stopwatch();
                _logger.LogInformation("Executing query", query);
                var result = await _queryBus.Execute(query, token).ConfigureAwait(false);
                stopwatch.Stop();
                _logger.LogInformation("Finished executing query", new {query, stopwatch.ElapsedMilliseconds});
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to execute query", query);
                throw;
            }
        }
    }
}