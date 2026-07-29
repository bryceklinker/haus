using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class ZigbeeToHausRelayTests
{
    [Fact]
    public async Task StartAsync_CoordinatorConnectFails_DoesNotThrow()
    {
        var coordinator = new FakeZigbeeCoordinator { ConnectShouldThrow = new InvalidOperationException("no port") };
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var relay = ResolveRelay(provider);

        var exception = await Record.ExceptionAsync(() => relay.StartAsync(CancellationToken.None));

        Assert.Null(exception);
        Assert.False(coordinator.IsConnected);
        Assert.True(coordinator.ConnectAttempts >= 1);

        await relay.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_CoordinatorConnectSucceeds_LeavesCoordinatorConnected()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var relay = ResolveRelay(provider);

        await relay.StartAsync(CancellationToken.None);

        Assert.True(coordinator.IsConnected);

        await relay.StopAsync(CancellationToken.None);
    }

    private static IHostedService ResolveRelay(IServiceProvider provider)
    {
        return provider.GetServices<IHostedService>().OfType<ZigbeeToHausRelay>().Single();
    }
}
