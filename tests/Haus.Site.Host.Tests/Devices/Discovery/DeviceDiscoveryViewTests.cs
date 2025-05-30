using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Rooms;
using Haus.Site.Host.Devices.Discovery;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Tests.Support.Realtime;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Haus.Site.Host.Tests.Devices.Discovery;

public class DeviceDiscoveryViewTests : HausSiteTestContext
{
    private const string RoomsUrl = "/api/rooms";
    private const string DevicesUrl = "/api/devices";
    private readonly InMemoryRealtimeDataSubscriber _devicesSubscriber;

    public DeviceDiscoveryViewTests()
    {
        _devicesSubscriber = GetSubscriber(HausRealtimeSources.Events);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>());
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>());
    }

    [Fact]
    public async Task WhenRenderedThenShowsLoading()
    {
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>(), opts => opts.WithDelayMs(1000));
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>(), opts => opts.WithDelayMs(1000));

        var page = Context.RenderComponent<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
        });
    }

    [Fact]
    public void WhenRenderedThenConnectsToEvents()
    {
        RenderView<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            _devicesSubscriber.IsStarted.Should().BeTrue();
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsAllRooms()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>());
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>(
                [
                    HausModelFactory.RoomModel() with
                    {
                        Name = "Basement",
                        Id = 4,
                    },
                    HausModelFactory.RoomModel() with
                    {
                        Name = "Master Bedroom",
                        Id = 3,
                    },
                ]
            )
        );

        var page = Context.RenderComponent<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudDropZone<DeviceModel>>().Should().HaveCount(3);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsAllUnassignedDevices()
    {
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>([HausModelFactory.RoomModel() with { Id = 6 }])
        );
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>(
                [
                    HausModelFactory.DeviceModel() with
                    {
                        RoomId = 6,
                    },
                    HausModelFactory.DeviceModel() with
                    {
                        RoomId = null,
                    },
                    HausModelFactory.DeviceModel() with
                    {
                        RoomId = null,
                    },
                    HausModelFactory.DeviceModel() with
                    {
                        RoomId = 6,
                    },
                ]
            )
        );

        var page = Context.RenderComponent<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudPaper>().Should().HaveCount(4);
        });
    }

    [Fact]
    public async Task WhenDeviceIsPlacedInRoomThenDeviceIsAssignedToRoom()
    {
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>([HausModelFactory.RoomModel() with { Id = 6, Name = "bathroom" }])
        );
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>([HausModelFactory.DeviceModel() with { RoomId = null, Id = 76 }])
        );
        HttpRequestMessage? postRequest = null;
        await HausApiHandler.SetupPostAsJson(
            $"{RoomsUrl}/{6}/add-devices",
            new { },
            opts => opts.WithCapture(r => postRequest = r)
        );

        var page = Context.RenderComponent<DeviceDiscoveryView>();
        var unAssignedZone = page.FindByComponent<MudDropZone<DeviceModel>>(opts =>
            opts.WithText("unassigned devices")
        );

        var device = unAssignedZone.FindByTag("div", opts => opts.WithClassName("device"));
        await device.DragStartAsync(new DragEventArgs());

        var bathroomZone = page.FindByComponent<MudDropZone<DeviceModel>>(opts => opts.WithText("bathroom"));
        await bathroomZone.FindByTag("div").DropAsync(new DragEventArgs());

        await Eventually.AssertAsync(async () =>
        {
            var content = postRequest?.Content != null ? await postRequest.Content.ReadFromJsonAsync<long[]>() : [];
            content.Should().Contain(76L);
        });
    }

    [Fact]
    public async Task WhenDeviceIsDiscoveredThenShowsDeviceInUnassignedDevices()
    {
        var device = HausModelFactory.DeviceModel();
        var view = Context.RenderComponent<DeviceDiscoveryView>();
        await _devicesSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new DeviceCreatedEvent(device).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudPaper>(opts => opts.WithText(device.ExternalId)).Should().HaveCount(1);
        });
    }
}
