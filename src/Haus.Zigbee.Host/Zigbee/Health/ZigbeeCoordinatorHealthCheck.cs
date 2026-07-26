using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Zigbee.Host.Zigbee.Health;

public class ZigbeeCoordinatorHealthCheck(IZigbeeCoordinator coordinator) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new()
    )
    {
        return Task.FromResult(coordinator.IsConnected ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }
}
