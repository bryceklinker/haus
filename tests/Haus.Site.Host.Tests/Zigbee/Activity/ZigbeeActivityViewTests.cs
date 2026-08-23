using System.Linq;
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

    [Fact]
    public async Task WhenNoActivityThenShowsEmptyMessage()
    {
        await HausApiHandler.SetupGetAsJson(ActivityUrl, new ListResult<ZigbeeActivityEntryModel>());

        var view = RenderView<ZigbeeActivityView>();

        Eventually.Assert(() =>
        {
            view.FindByComponent<MudText>(opts => opts.WithText("No recent zigbee activity"));
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsInitialActivityNewestFirst()
    {
        var older = HausModelFactory.ZigbeeActivityEntryModel() with { EventType = "older" };
        var newer = HausModelFactory.ZigbeeActivityEntryModel() with { EventType = "newer" };
        await HausApiHandler.SetupGetAsJson(ActivityUrl, new ListResult<ZigbeeActivityEntryModel>([older, newer]));

        var view = RenderView<ZigbeeActivityView>();

        Eventually.Assert(() =>
        {
            var entries = view.FindAllByComponent<ZigbeeActivityEntryView>().ToArray();
            Assert.Equal(2, entries.Length);
            Assert.Equal("newer", entries[0].Instance.Entry?.EventType);
            Assert.Equal("older", entries[1].Instance.Entry?.EventType);
        });
    }
}
