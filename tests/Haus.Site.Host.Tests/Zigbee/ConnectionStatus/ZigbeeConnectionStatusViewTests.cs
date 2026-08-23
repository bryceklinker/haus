using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Site.Host.Shared.Theming;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Site.Host.Zigbee.ConnectionStatus;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.ConnectionStatus;

public class ZigbeeConnectionStatusViewTests : HausSiteTestContext
{
    private const string StatusUrl = "/api/zigbee/status";
    private static readonly HausTheme Theme = new();
    private readonly InMemoryRealtimeDataSubscriber _eventsSubscriber;

    public ZigbeeConnectionStatusViewTests()
    {
        _eventsSubscriber = GetSubscriber(HausRealtimeSources.Events);
    }

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

    [Fact]
    public async Task WhenConnectedThenShowsConnectedBanner()
    {
        var status = HausModelFactory.ZigbeeConnectionStatusModel() with { IsConnected = true };
        await HausApiHandler.SetupGetAsJson(StatusUrl, status);

        var view = RenderView<ZigbeeConnectionStatusView>();

        Eventually.Assert(() =>
        {
            var banner = view.FindByComponent<MudText>(opts => opts.WithText("Connected"));
            Assert.Contains($"background-color: {Theme.PaletteLight.Success.Value}", banner.Instance.Style);
        });
    }

    [Fact]
    public async Task WhenDisconnectedThenShowsDisconnectedBannerWithReason()
    {
        var status = HausModelFactory.ZigbeeConnectionStatusModel() with
        {
            IsConnected = false,
            Reason = "serial port hung",
        };
        await HausApiHandler.SetupGetAsJson(StatusUrl, status);

        var view = RenderView<ZigbeeConnectionStatusView>();

        Eventually.Assert(() =>
        {
            var banner = view.FindByComponent<MudText>(opts => opts.WithText("Disconnected"));
            Assert.Contains($"background-color: {Theme.PaletteLight.Error.Value}", banner.Instance.Style);
            view.FindByComponent<MudText>(opts => opts.WithText("serial port hung"));
        });
    }

    [Fact]
    public async Task WhenStatusIsUnknownThenShowsUnknownBanner()
    {
        await HausApiHandler.SetupGetAsJson(StatusUrl, ZigbeeConnectionStatusModel.Unknown);

        var view = RenderView<ZigbeeConnectionStatusView>();

        Eventually.Assert(() =>
        {
            var banner = view.FindByComponent<MudText>(opts => opts.WithText("Unknown"));
            Assert.Contains($"background-color: {Theme.PaletteLight.Info.Value}", banner.Instance.Style);
        });
    }

    [Fact]
    public async Task WhenConnectionStatusChangedEventReceivedThenUpdatesBannerLive()
    {
        await HausApiHandler.SetupGetAsJson(
            StatusUrl,
            HausModelFactory.ZigbeeConnectionStatusModel() with
            {
                IsConnected = false,
            }
        );
        var view = RenderView<ZigbeeConnectionStatusView>();
        Eventually.Assert(() =>
        {
            view.FindByComponent<MudText>(opts => opts.WithText("Disconnected"));
        });

        await _eventsSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new ZigbeeConnectionStatusChangedEvent(true, null, null).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            var banner = view.FindByComponent<MudText>(opts => opts.WithText("Connected"));
            Assert.Contains($"background-color: {Theme.PaletteLight.Success.Value}", banner.Instance.Style);
        });
    }
}
