using System.Threading.Tasks;
using AngleSharp;
using Haus.Core.Models;
using Haus.Core.Models.Health;
using Haus.Site.Host.Health.HealthStatus;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.Health.HealthStatus;

public class HealthStatusViewTests : HausSiteTestContext
{
    private readonly InMemoryRealtimeDataSubscriber _healthSubscriber;

    public HealthStatusViewTests()
    {
        _healthSubscriber = GetSubscriber(HausRealtimeSources.Health);
    }

    [Fact]
    public void WhenRenderedThenConnectsToRealtimeHealth()
    {
        RenderView<HealthStatusView>();

        Eventually.Assert(() =>
        {
            _healthSubscriber.IsStarted.Should().BeTrue();
        });
    }

    [Fact]
    public void WhenConnectingToHubThenLoadingIsShown()
    {
        _healthSubscriber.ConfigureStartDelayMs(500);

        var view = RenderView<HealthStatusView>();

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
        });
    }

    [Theory]
    [InlineData(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy, "#4caf50ff")]
    [InlineData(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, "#f44336ff")]
    [InlineData(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded, "#ff5722ff")]
    public async Task WhenRenderedThenShowsTheLatestHealthStatus(
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus status,
        string color
    )
    {
        var report = HausModelFactory.HealthReportModel() with { Status = status };

        var view = RenderView<HealthStatusView>();
        await _healthSubscriber.SimulateAsync(HausHealthEventNames.OnHealth, report);

        Eventually.Assert(() =>
        {
            var paper = view.FindByComponent<MudText>(opts => opts.WithText($"{report.Status}"));
            paper.Instance.Style.Should().Contain($"background-color: {color}");
        });
    }

    [Fact]
    public async Task WhenHealthReportIsAvailableThenShowsEachCheck()
    {
        var report = HausModelFactory.HealthReportModel() with
        {
            Checks =
            [
                HausModelFactory.HealthCheckModel(),
                HausModelFactory.HealthCheckModel(),
                HausModelFactory.HealthCheckModel(),
            ],
        };

        var view = RenderView<HealthStatusView>();
        await _healthSubscriber.SimulateAsync(HausHealthEventNames.OnHealth, report);

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudCollapse>().Should().HaveCount(3);
        });
    }
}
