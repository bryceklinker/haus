using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Health;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Health;

public class ZigbeeCoordinatorHealthCheckTests
{
    private readonly FakeZigbeeCoordinator _coordinator;
    private readonly ZigbeeCoordinatorHealthCheck _healthCheck;

    public ZigbeeCoordinatorHealthCheckTests()
    {
        _coordinator = new FakeZigbeeCoordinator();

        _healthCheck = ServiceProviderFactory.Create(
            zigbeeCoordinator: _coordinator
        ).GetRequiredService<ZigbeeCoordinatorHealthCheck>();
    }
    
    [Fact]
    public async Task CheckHealthAsync_CoordinatorConnected_ReturnsHealthy()
    {
        _coordinator.IsConnected = true;

        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_CoordinatorNotConnected_ReturnsUnhealthy()
    {
        _coordinator.IsConnected = false;

        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
