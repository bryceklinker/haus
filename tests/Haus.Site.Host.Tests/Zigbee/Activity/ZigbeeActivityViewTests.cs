using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Zigbee.Activity;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.Activity;

public class ZigbeeActivityViewTests : HausSiteTestContext
{
    private const string ActivityUrl = "/api/zigbee/activity";

    [Fact]
    public async Task WhenRenderedThenShowsLoadingActivity()
    {
        await HausApiHandler.SetupGetAsJson(
            ActivityUrl,
            new ListResult<ZigbeeActivityEntryModel>(),
            opts => opts.WithDelayMs(1000)
        );

        var view = RenderView<ZigbeeActivityView>();

        Assert.Single(view.FindAllByComponent<MudProgressCircular>());
    }
}
