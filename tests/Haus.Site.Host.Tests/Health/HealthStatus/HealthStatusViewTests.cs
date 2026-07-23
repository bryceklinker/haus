using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Health;
using Haus.Site.Host.Health.HealthStatus;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
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
            Assert.True(_healthSubscriber.IsStarted);
        });
    }

    [Fact]
    public void WhenConnectingToHubThenLoadingIsShown()
    {
        _healthSubscriber.ConfigureStartDelayMs(500);

        var view = RenderView<HealthStatusView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(1, view.FindAllByComponent<MudProgressCircular>().Count());
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
            Assert.Contains($"background-color: {color}", paper.Instance.Style);
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
            Assert.Equal(3, view.FindAllByComponent<MudCollapse>().Count());
        });
    }
}
