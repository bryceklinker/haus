using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Health;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Health;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mqtt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Health;

public class ZigbeeHostHealthPublisherTests : IAsyncLifetime
{
    private ZigbeeHostHealthPublisher _publisher;
    private IHausMqttClient _mqttClient;

    public async Task InitializeAsync()
    {
        var provider = ServiceProviderFactory.Create(mqttFactory: new FakeMqttClientFactory());
        var zigbeeMqttFactory = provider.GetRequiredService<IZigbeeMqttClientFactory>();
        _mqttClient = await zigbeeMqttFactory.CreateHausClient();

        var hausOptions = provider.GetRequiredService<IOptions<HausOptions>>();
        _publisher = new ZigbeeHostHealthPublisher(zigbeeMqttFactory, hausOptions);
    }

    [Fact]
    public async Task WhenReceivesHealthyReportThenPublishesHealthReportToMqtt()
    {
        HausHealthReportModel actual = null;
        await _mqttClient.SubscribeToHausHealthAsync(r => actual = r);

        var report = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Healthy,
            TimeSpan.FromMilliseconds(200));
        await _publisher.PublishAsync(report);

        Eventually.Assert(() =>
        {
            actual.IsOk.Should().BeTrue();
            actual.Status.Should().Be(HealthStatus.Healthy);
        });
    }

    public async Task DisposeAsync()
    {
        await _mqttClient.DisposeAsync();
    }
}