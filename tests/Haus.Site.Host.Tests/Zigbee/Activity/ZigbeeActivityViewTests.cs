using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Site.Host.Zigbee.Activity;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.Activity;

public class ZigbeeActivityViewTests : HausSiteTestContext
{
    private const string ActivityUrl = "/api/zigbee/activity";
    private readonly InMemoryRealtimeDataSubscriber _eventsSubscriber;

    public ZigbeeActivityViewTests()
    {
        _eventsSubscriber = GetSubscriber(HausRealtimeSources.Events);
    }

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

    [Fact]
    public async Task WhenZigbeeEventReceivedThenPrependsToActivityFeed()
    {
        await HausApiHandler.SetupGetAsJson(ActivityUrl, new ListResult<ZigbeeActivityEntryModel>());
        var view = RenderView<ZigbeeActivityView>();
        Eventually.Assert(() =>
        {
            view.FindByComponent<MudText>(opts => opts.WithText("No recent zigbee activity"));
        });

        await _eventsSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new ZigbeeDeviceJoinedEvent("00:11:22:33:44:55:66:77", 1234).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            var entry = view.FindByComponent<ZigbeeActivityEntryView>();
            Assert.Equal(ZigbeeDeviceJoinedEvent.Type, entry.Instance.Entry?.EventType);
        });
    }

    [Fact]
    public async Task WhenActivityExceedsMaxThenOldestEntriesAreDropped()
    {
        var initial = Enumerable.Range(0, 100).Select(_ => HausModelFactory.ZigbeeActivityEntryModel()).ToArray();
        await HausApiHandler.SetupGetAsJson(ActivityUrl, new ListResult<ZigbeeActivityEntryModel>(initial));
        var view = RenderView<ZigbeeActivityView>();
        Eventually.Assert(() =>
        {
            Assert.Equal(100, view.FindAllByComponent<ZigbeeActivityEntryView>().Count());
        });

        await _eventsSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new ZigbeeDeviceJoinedEvent("00:11:22:33:44:55:66:77", 1234).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            Assert.Equal(100, view.FindAllByComponent<ZigbeeActivityEntryView>().Count());
        });
    }
}
