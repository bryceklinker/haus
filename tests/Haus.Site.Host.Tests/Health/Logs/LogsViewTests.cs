using System;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Logs;
using Haus.Site.Host.Health.Logs;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Health.Logs;

public class LogsViewTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenRenderedAndLoadingLogsThenShowsLoading()
    {
        await HausApiHandler.SetupGetAsJson(
            "/api/logs",
            new ListResult<LogEntryModel>([]),
            opts => opts.WithDelayMs(1000)
        );

        var view = RenderView<LogsView>();

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
        });
    }

    [Fact]
    public async Task WhenRenderedThenGetsLogsFromApi()
    {
        await HausApiHandler.SetupGetAsJson(
            "/api/logs",
            new ListResult<LogEntryModel>(
                [HausModelFactory.LogEntryModel(), HausModelFactory.LogEntryModel(), HausModelFactory.LogEntryModel()]
            )
        );

        var view = RenderView<LogsView>();

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<LogEntryView>().Should().HaveCount(3);
        });
    }

    [Fact]
    public async Task WhenRenderedThenRefreshesLogsOnProvidedInterval()
    {
        var requestCount = 0;
        await HausApiHandler.SetupGetAsJson(
            "/api/logs",
            new ListResult<LogEntryModel>([]),
            opts => opts.WithCapture(r => requestCount++)
        );

        RenderView<LogsView>(opts =>
        {
            opts.Add(c => c.RefreshInterval, TimeSpan.FromMilliseconds(200));
        });

        Eventually.Assert(() =>
        {
            requestCount.Should().BeGreaterThan(1);
        });
    }
}
