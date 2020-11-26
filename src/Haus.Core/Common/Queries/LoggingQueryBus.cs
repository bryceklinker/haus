using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Common.Queries
{
    internal class LoggingQueryBus : LoggingBus, IQueryBus
    {
        private readonly IQueryBus _queryBus;

        protected override string BeginMessage => "Executing query";
        protected override string FinishedMessage => "Finished executing query";
        protected override string ErrorMessage => "Error executing query";

        public LoggingQueryBus(IQueryBus queryBus, ILogger<LoggingQueryBus> logger)
            : base(logger)
        {
            _queryBus = queryBus;
        }

        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
        {
            return await ExecuteWithLoggingAsync(query, () => _queryBus.ExecuteAsync(query, token), token)
                .ConfigureAwait(false);
        }
    }
}