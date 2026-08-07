using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Logs;
using Haus.Site.Host.Health;
using Haus.Site.Host.Health.Events;
using Haus.Site.Host.Health.Logs;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
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
            Assert.True(subscriber.IsStarted);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsLogsView()
    {
        await SetupHealthApis();
        var view = RenderView<HealthView>();

        Eventually.Assert(() =>
        {
            Assert.Single(view.FindAllByComponent<LogsView>());
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsEventsView()
    {
        await SetupHealthApis();
        var view = RenderView<HealthView>();

        Eventually.Assert(() =>
        {
            Assert.Single(view.FindAllByComponent<EventsView>());
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
            Assert.EndsWith("/health/diagnostics", NavigationManager.Uri);
        });
    }

    private async Task SetupHealthApis()
    {
        await HausApiHandler.SetupGetAsJson("/api/logs", new ListResult<LogEntryModel>());
    }
}
