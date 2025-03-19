using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Logs;
using Haus.Site.Host.Health;
using Haus.Site.Host.Health.Logs;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;

namespace Haus.Site.Host.Tests.Health;

public class HealthViewTests : HausSiteTestContext
{
    [Fact]
    public async Task WhenRenderedThenShowsCurrentHealth()
    {
        await SetupHealthApis();
        var hub = GetSubscriber(HausRealtimeSources.Health);

        RenderView<HealthView>();

        Eventually.Assert(() =>
        {
            hub.IsStarted.Should().BeTrue();
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

    private async Task SetupHealthApis()
    {
        await HausApiHandler.SetupGetAsJson("/api/logs", new ListResult<LogEntryModel>());
    }
}
