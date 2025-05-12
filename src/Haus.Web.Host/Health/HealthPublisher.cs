using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Health.Queries;
using Haus.Core.Models.Health;
using Haus.Cqrs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Haus.Web.Host.Health;

public class HealthPublisher(
    ILogger<HealthPublisher> logger,
    ILastKnownHealthCache lastKnownHealthCache,
    IServiceScopeFactory scopeFactory
) : IHealthCheckPublisher
{
    private readonly ILogger _logger = logger;

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var storedChecks = await GetStoredHealthChecks(scope).ConfigureAwait(false);
        var health = HausHealthReportModel.FromHealthReport(report).AppendChecks(storedChecks);

        if (health.IsError)
            _logger.LogCritical("System is reporting to be down");
        else if (health.IsOk)
            _logger.LogInformation("System is reporting to be healthy");
        else if (health.IsWarn)
            _logger.LogWarning("System is report to be have warnings");

        lastKnownHealthCache.UpdateLatestReport(health);
        await PublishHealthReport(scope, health);
    }

    private async Task<HausHealthCheckModel[]> GetStoredHealthChecks(IServiceScope scope)
    {
        var bus = scope.GetService<IHausBus>();
        var result = await bus.ExecuteQueryAsync(new GetAllHealthChecksQuery()).ConfigureAwait(false);
        return result.Items;
    }

    private static async Task PublishHealthReport(IServiceScope scope, HausHealthReportModel health)
    {
        var hubContext = scope.GetService<IHubContext<HealthHub>>();
        await hubContext.BroadcastAsync("OnHealth", health).ConfigureAwait(false);
    }
}
