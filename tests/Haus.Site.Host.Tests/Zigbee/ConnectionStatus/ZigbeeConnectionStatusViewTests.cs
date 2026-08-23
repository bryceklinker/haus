using System.Threading.Tasks;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Zigbee.ConnectionStatus;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.ConnectionStatus;

public class ZigbeeConnectionStatusViewTests : HausSiteTestContext
{
    private const string StatusUrl = "/api/zigbee/status";

    [Fact]
    public async Task WhenRenderedThenShowsLoadingWhileFetchingStatus()
    {
        await HausApiHandler.SetupGetAsJson(
            StatusUrl,
            HausModelFactory.ZigbeeConnectionStatusModel(),
            opts => opts.WithDelayMs(1000)
        );

        var view = RenderView<ZigbeeConnectionStatusView>();

        Assert.Single(view.FindAllByComponent<MudProgressCircular>());
    }
}
