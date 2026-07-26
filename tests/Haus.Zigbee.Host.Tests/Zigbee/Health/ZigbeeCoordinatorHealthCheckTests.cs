using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Health;

public class ZigbeeCoordinatorHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_CoordinatorConnected_ReturnsHealthy()
    {
        var coordinator = new FakeZigbeeCoordinator { IsConnected = true };
        var healthCheck = new ZigbeeCoordinatorHealthCheck(coordinator);

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_CoordinatorNotConnected_ReturnsUnhealthy()
    {
        var coordinator = new FakeZigbeeCoordinator { IsConnected = false };
        var healthCheck = new ZigbeeCoordinatorHealthCheck(coordinator);

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
