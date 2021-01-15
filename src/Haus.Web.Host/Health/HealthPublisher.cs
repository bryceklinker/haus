using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Health;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Haus.Web.Host.Health
{
    public class HealthPublisher : IHealthCheckPublisher
    {
        private readonly ILogger _logger;
        private readonly ILastKnownHealthCache _lastKnownHealthCache;
        private readonly IServiceScopeFactory _scopeFactory;

        public HealthPublisher(ILogger<HealthPublisher> logger, ILastKnownHealthCache lastKnownHealthCache, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _lastKnownHealthCache = lastKnownHealthCache;
            _scopeFactory = scopeFactory;
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var health = HausHealthReportModel.FromHealthReport(report);
            if (health.IsError) _logger.LogCritical("System is reporting to be down");
            else if (health.IsOk) _logger.LogInformation("System is reporting to be healthy");

            _lastKnownHealthCache.LastKnownHealth = health;
            using var scope = _scopeFactory.CreateScope();
            var hubContext = scope.GetService<IHubContext<HealthHub>>();
            await hubContext.BroadcastAsync("OnHealth", health).ConfigureAwait(false);
        }
    }
}