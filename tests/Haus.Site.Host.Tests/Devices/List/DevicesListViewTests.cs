using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom.Events;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Site.Host.Devices.Detail;
using Haus.Site.Host.Devices.List;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Haus.Site.Host.Tests.Devices.List;

public class DevicesListViewTests : HausSiteTestContext
{
    private const string DevicesUrl = "/api/devices";
    private readonly InMemoryRealtimeDataSubscriber _eventsSubscriber;

    public DevicesListViewTests()
    {
        _eventsSubscriber = GetSubscriber(HausRealtimeSources.Events);
    }

    [Fact]
    public async Task WhenRenderedThenShowsLoadingDevices()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>(), opts => opts.WithDelayMs(1000));

        var page = RenderView<DevicesListView>();

        Assert.Single(page.FindAllByComponent<MudProgressCircular>());
    }

    [Fact]
    public async Task WhenRenderedThenShowsListOfDevices()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>([
                HausModelFactory.DeviceModel(),
                HausModelFactory.DeviceModel(),
                HausModelFactory.DeviceModel(),
            ])
        );

        var page = RenderView<DevicesListView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(3, page.FindAllByComponent<MudListItem<DeviceModel>>().Count());
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsNameAndTypeOfDevice()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>([
                HausModelFactory.DeviceModel() with
                {
                    DeviceType = DeviceType.MotionSensor,
                    Name = "Motions",
                },
            ])
        );

        var page = RenderView<DevicesListView>();

        Eventually.Assert(() =>
        {
            var item = page.FindByComponent<MudListItem<DeviceModel>>();
            Assert.Contains("Motions", item.Instance.Text);
            Assert.Contains("Motion Sensor", item.Instance.SecondaryText);
        });
    }

    [Fact]
    public async Task WhenDeviceSelectedThenShowsDeviceDetails()
    {
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>([
                HausModelFactory.DeviceModel() with
                {
                    Id = 5,
                    DeviceType = DeviceType.MotionSensor,
                    Name = "Motions",
                },
            ])
        );

        var page = RenderView<DevicesListView>();
        var deviceItem = page.FindByComponent<MudListItem<DeviceModel>>();
        await page.InvokeAsync(async () =>
        {
            await deviceItem.Instance.OnClick.InvokeAsync(new MouseEventArgs());
        });

        Eventually.Assert(() =>
        {
            var navigation = page.Services.GetRequiredService<BunitNavigationManager>();
            Assert.EndsWith("/devices/5", navigation.Uri);
        });
    }

    [Fact]
    public async Task WhenDiscoveryIsTriggeredThenNavigatesToDiscoveryPage()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>());

        var view = RenderView<DevicesListView>();

        Eventually.Assert(() =>
        {
            var navLink = view.FindByComponent<MudNavLink>(opts => opts.WithText("discovery"));
            Assert.Contains("/devices/discovery", navLink.Instance.Href);
        });
    }

    [Fact]
    public async Task WhenDeviceDeletedEventReceivedThenRemovesDeviceFromList()
    {
        var device = HausModelFactory.DeviceModel() with { Id = 5 };
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>([device]));

        var page = RenderView<DevicesListView>();
        Eventually.Assert(() =>
        {
            Assert.Single(page.FindAllByComponent<MudListItem<DeviceModel>>());
        });

        await _eventsSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new DeviceDeletedEvent(device).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            Assert.Empty(page.FindAllByComponent<MudListItem<DeviceModel>>());
        });
    }

    [Fact]
    public async Task WhenSelectedDeviceIsDeletedThenNavigatesToDevicesList()
    {
        var device = HausModelFactory.DeviceModel() with { Id = 5 };
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>([device]));

        var page = RenderView<DevicesListView>(opts => opts.Add(p => p.DeviceId, 5L));
        Eventually.Assert(() =>
        {
            Assert.NotEmpty(page.FindAllByComponent<DeviceDetailView>());
        });

        await _eventsSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new DeviceDeletedEvent(device).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            var navigation = page.Services.GetRequiredService<BunitNavigationManager>();
            Assert.EndsWith("/devices", navigation.Uri);
        });
    }
}
