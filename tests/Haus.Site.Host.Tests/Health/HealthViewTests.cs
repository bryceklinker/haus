using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Logs;
using Haus.Site.Host.Health;
using Haus.Site.Host.Health.Events;
using Haus.Site.Host.Health.Logs;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;

namespace Haus.Site.Host.Tests.Health;

public class HealthViewTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenRenderedThenShowsCurrentHealth()
    {
        await SetupHealthApis();
        var subscriber = GetSubscriber(HausRealtimeSources.Health);

        RenderView<HealthView>();

        Eventually.Assert(() =>
        {
            subscriber.IsStarted.Should().BeTrue();
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsLogsView()
    {
        await SetupHealthApis();
        var view = RenderView<HealthView>();

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<LogsView>().Should().HaveCount(1);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsEventsView()
    {
        await SetupHealthApis();
        var view = RenderView<HealthView>();

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<EventsView>().Should().HaveCount(1);
        });
    }

    [Fact]
    public async Task WhenNavigatingToDiagnosticsThenNavigatesUserToDiagnostics()
    {
        await SetupHealthApis();
        var view = RenderView<HealthView>();

        await view.FindByComponent<MudButton>(opts => opts.WithText("Diagnostics")).ClickAsync();

        Eventually.Assert(() =>
        {
            NavigationManager.Uri.Should().EndWith("/health/diagnostics");
        });
    }

    private async Task SetupHealthApis()
    {
        await HausApiHandler.SetupGetAsJson("/api/logs", new ListResult<LogEntryModel>());
    }
}
