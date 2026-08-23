using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Site.Host.Zigbee.Devices;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.Devices;

public class ZigbeeDevicesViewTests : HausSiteTestContext
{
    private const string DevicesUrl = "/api/zigbee/devices";
    private readonly InMemoryRealtimeDataSubscriber _eventsSubscriber;

    public ZigbeeDevicesViewTests()
    {
        _eventsSubscriber = GetSubscriber(HausRealtimeSources.Events);
    }

    [Fact]
    public async Task WhenRenderedThenShowsLoadingDevices()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<ZigbeeKnownDeviceModel>(),
            opts => opts.WithDelayMs(1000)
        );

        var view = RenderView<ZigbeeDevicesView>();

        Assert.Single(view.FindAllByComponent<MudProgressCircular>());
    }

    [Fact]
    public async Task WhenNoDevicesThenShowsEmptyMessage()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<ZigbeeKnownDeviceModel>());

        var view = RenderView<ZigbeeDevicesView>();

        Eventually.Assert(() =>
        {
            view.FindByComponent<MudText>(opts => opts.WithText("No zigbee devices discovered yet"));
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsEachKnownDevice()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<ZigbeeKnownDeviceModel>([
                HausModelFactory.ZigbeeKnownDeviceModel(),
                HausModelFactory.ZigbeeKnownDeviceModel(),
                HausModelFactory.ZigbeeKnownDeviceModel(),
            ])
        );

        var view = RenderView<ZigbeeDevicesView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(3, view.FindAllByComponent<ZigbeeDeviceView>().Count());
        });
    }

    [Fact]
    public async Task WhenDeviceJoinedEventReceivedThenRefreshesDevicesList()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<ZigbeeKnownDeviceModel>());
        var view = RenderView<ZigbeeDevicesView>();
        Eventually.Assert(() =>
        {
            view.FindByComponent<MudText>(opts => opts.WithText("No zigbee devices discovered yet"));
        });

        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<ZigbeeKnownDeviceModel>([HausModelFactory.ZigbeeKnownDeviceModel()])
        );
        await _eventsSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new ZigbeeDeviceJoinedEvent("00:11:22:33:44:55:66:77", 1234).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            Assert.Single(view.FindAllByComponent<ZigbeeDeviceView>());
        });
    }
}
