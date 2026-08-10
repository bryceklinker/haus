using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Discovery;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class ZigbeeHausBridgeTests
{
    [Fact]
    public async Task HandleHausCommand_OutboundRelayThrows_SubsequentCommandsStillGetHandled()
    {
        var coordinator = new FakeZigbeeCoordinator
        {
            SetPermitJoinShouldThrow = new InvalidOperationException("boom"),
        };
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);

        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        var hausMqttClient = await mqttClientFactory.CreateClient();

        var badMessage = new StartDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");
        await hausMqttClient.PublishAsync(badMessage);

        coordinator.SetPermitJoinShouldThrow = null;
        var goodMessage = new StopDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");
        await hausMqttClient.PublishAsync(goodMessage);

        Eventually.Assert(() => Assert.Contains(false, coordinator.PermitJoinCalls));
        Assert.DoesNotContain(true, coordinator.PermitJoinCalls);

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_CoordinatorConnectFails_DoesNotThrow()
    {
        var coordinator = new FakeZigbeeCoordinator { ConnectShouldThrow = new InvalidOperationException("no port") };
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);

        var exception = await Record.ExceptionAsync(() => bridge.StartAsync(CancellationToken.None));

        Assert.Null(exception);
        Assert.False(coordinator.IsConnected);
        Assert.True(coordinator.ConnectAttempts >= 1);

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_CoordinatorConnectSucceeds_LeavesCoordinatorConnected()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);

        await bridge.StartAsync(CancellationToken.None);

        Assert.True(coordinator.IsConnected);

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StopAsync_CalledWithoutStartAsyncHavingSetTheMqttClient_DoesNotThrow()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);

        var exception = await Record.ExceptionAsync(() => bridge.StopAsync(CancellationToken.None));

        Assert.Null(exception);
    }

    private static IHostedService ResolveBridge(IServiceProvider provider)
    {
        return provider.GetServices<IHostedService>().OfType<ZigbeeHausBridge>().Single();
    }
}
