using System.Threading.Tasks;
using AngleSharp;
using Haus.Core.Models;
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
    public void WhenRenderedThenConnectsToHealthHub()
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

    [Fact]
    public async Task WhenRenderedThenShowsTheLatestHealthStatus()
    {
        var report = HausModelFactory.HealthReportModel();

        var view = RenderView<HealthStatusView>();
        await _healthSubscriber.SimulateAsync("OnHealth", report);

        Eventually.Assert(() =>
        {
            view.FindByClass("health-status").TextContent.Should().Contain($"{report.Status}");
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
        await _healthSubscriber.SimulateAsync("OnHealth", report);

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudCollapse>().Should().HaveCount(3);
        });
    }

    [Fact]
    public async Task WhenHealthCheckIsExpandedThenShowsHealthCheckDetails()
    {
        var check = HausModelFactory.HealthCheckModel() with
        {
            Description = "hello",
            DurationOfCheckInMilliseconds = 9000,
            ExceptionMessage = "bad things",
        };
        var report = HausModelFactory.HealthReportModel() with { Checks = [check] };
        var view = RenderView<HealthStatusView>();
        await _healthSubscriber.SimulateAsync("OnHealth", report);

        await view.InvokeAsync(async () =>
        {
            await view.FindByComponent<MudButton>().ClickAsync();
        });

        Eventually.Assert(() =>
        {
            view.FindByComponent<MudCollapse>().Instance.Expanded.Should().BeTrue();
            view.FindComponent<MudButton>().Nodes.ToHtml().Should().Contain(check.Name);
            view.FindMudTextFieldById<string>("description").GetValue().Should().Be("hello");
            view.FindMudTextFieldById<double>("duration").GetValue().Should().Be(9);
            view.FindMudTextFieldById<string>("error").GetValue().Should().Be("bad things");
        });
    }
}
