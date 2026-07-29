using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Health;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Host.Health;
using Haus.Zigbee.Host.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Health;

public class ZigbeeHostHealthPublisherTests : IAsyncLifetime
{
    private IHealthCheckPublisher? _publisher;
    private IHausMqttClient? _mqttClient;
    private FakeZigbeeCoordinator? _coordinator;

    public async Task InitializeAsync()
    {
        _coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: _coordinator
        );
        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _mqttClient = await mqttClientFactory.CreateClient();

        _publisher = provider.GetRequiredService<IHealthCheckPublisher>();
    }

    public async Task DisposeAsync()
    {
        await _mqttClient!.DisposeAsync();
    }

    private static HealthReport EmptyReport()
    {
        return new HealthReport(
            new Dictionary<string, HealthReportEntry>(),
            HealthStatus.Healthy,
            TimeSpan.FromMilliseconds(200)
        );
    }

    [Fact]
    public async Task PublishAsync_CoordinatorConnected_PublishesHealthyZigbeeCheck()
    {
        _coordinator!.IsConnected = true;
        HausHealthReportModel? actual = null;
        await _mqttClient!.SubscribeToHausHealthAsync(r => actual = r);

        await _publisher!.PublishAsync(EmptyReport(), CancellationToken.None);

        Eventually.Assert(
            () => Assert.Contains(actual!.Checks, c => c.Name == "Zigbee" && c.Status == HealthStatus.Healthy)
        );
    }

    [Fact]
    public async Task PublishAsync_CoordinatorNotConnected_PublishesUnhealthyZigbeeCheck()
    {
        _coordinator!.IsConnected = false;
        HausHealthReportModel? actual = null;
        await _mqttClient!.SubscribeToHausHealthAsync(r => actual = r);

        await _publisher!.PublishAsync(EmptyReport(), CancellationToken.None);

        Eventually.Assert(
            () => Assert.Contains(actual!.Checks, c => c.Name == "Zigbee" && c.Status == HealthStatus.Unhealthy)
        );
    }
}
