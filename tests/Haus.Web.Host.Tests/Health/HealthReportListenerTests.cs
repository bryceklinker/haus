using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Common;
using Haus.Core.Models.Health;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Haus.Web.Host.Tests.Health;

[Collection(HausWebHostCollectionFixture.Name)]
public class ExternalHealthListenerTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenHealthReportReceivedThenReportChecksAreInHausHealthReport()
    {
        HausHealthReportModel report = null;
        var hub = await factory.CreateHubConnection("health");
        hub.On<HausHealthReportModel>("OnHealth", r => report = r);

        var mqttClient = await factory.GetMqttClient();
        var publishedChecks = new[]
        {
            new HausHealthCheckModel("External", HealthStatus.Healthy, 66, "External Check", null, []),
        };
        await mqttClient.PublishAsync(
            DefaultHausMqttTopics.HealthTopic,
            new HausHealthReportModel(HealthStatus.Healthy, 55, publishedChecks)
        );

        Eventually.Assert(() =>
        {
            report.Checks.Should().Contain(c => c.Name == "External");
        });
    }
}
