using System.Linq;
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

        var page = Context.Render<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            Assert.Single(page.FindAllByComponent<MudProgressCircular>());
        });
    }

    [Fact]
    public void WhenRenderedThenConnectsToEvents()
    {
        RenderView<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            Assert.True(_devicesSubscriber.IsStarted);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsAllRooms()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>());
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>([
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
            ])
        );

        var page = Context.Render<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(3, page.FindAllByComponent<MudDropZone<DeviceModel>>().Count());
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
            new ListResult<DeviceModel>([
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
            ])
        );

        var page = Context.Render<DeviceDiscoveryView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(4, page.FindAllByComponent<MudPaper>().Count());
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

        var page = Context.Render<DeviceDiscoveryView>();
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
            Assert.Contains(76L, content ?? []);
        });
    }

    [Fact]
    public async Task WhenDeviceIsDiscoveredThenShowsDeviceInUnassignedDevices()
    {
        var device = HausModelFactory.DeviceModel();
        var view = Context.Render<DeviceDiscoveryView>();
        await _devicesSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new DeviceCreatedEvent(device).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            Assert.Single(view.FindAllByComponent<MudPaper>(opts => opts.WithText(device.ExternalId)));
        });
    }

    [Fact]
    public async Task WhenDeviceIsDiscoveredWhileFetchingDevicesThenDiscoveredDeviceIsNotDropped()
    {
        var fetchedDevice = HausModelFactory.DeviceModel();
        var discoveredDevice = HausModelFactory.DeviceModel();
        await HausApiHandler.SetupGetAsJson(
            DevicesUrl,
            new ListResult<DeviceModel>([fetchedDevice]),
            opts => opts.WithDelayMs(500)
        );

        var view = Context.Render<DeviceDiscoveryView>();
        Eventually.Assert(() => Assert.True(_devicesSubscriber.IsStarted));

        await _devicesSubscriber.SimulateAsync(
            HausEventsEventNames.OnEvent,
            new DeviceCreatedEvent(discoveredDevice).AsHausEvent()
        );

        Eventually.Assert(() =>
        {
            Assert.Single(view.FindAllByComponent<MudPaper>(opts => opts.WithText(fetchedDevice.ExternalId)));
            Assert.Single(view.FindAllByComponent<MudPaper>(opts => opts.WithText(discoveredDevice.ExternalId)));
        });
    }

    [Fact]
    public async Task WhenDeviceDragStartsThenOtherDevicesCannotBeDraggedBeforeItIsDropped()
    {
        var deviceA = HausModelFactory.DeviceModel() with { Id = 78, RoomId = null };
        var deviceB = HausModelFactory.DeviceModel() with { Id = 79, RoomId = null };
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>([deviceA, deviceB]));

        var page = Context.Render<DeviceDiscoveryView>();
        Eventually.Assert(() => Assert.Equal(2, page.FindAllByComponent<MudPaper>().Count()));

        var deviceAElement = page.FindByTag("div", opts => opts.WithClassName("device").WithText(deviceA.ExternalId));
        await deviceAElement.DragStartAsync(new DragEventArgs());

        Eventually.Assert(() =>
        {
            var deviceBElement = page.FindByTag(
                "div",
                opts => opts.WithClassName("mud-drop-item").WithText(deviceB.ExternalId)
            );
            Assert.Equal("false", deviceBElement.GetAttribute("draggable"));
        });
    }

    [Fact]
    public async Task WhenDeviceDropIsInFlightThenOtherDevicesCannotBeDragged()
    {
        var deviceA = HausModelFactory.DeviceModel() with { Id = 76, RoomId = null };
        var deviceB = HausModelFactory.DeviceModel() with { Id = 77, RoomId = null };
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>([HausModelFactory.RoomModel() with { Id = 6, Name = "bathroom" }])
        );
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>([deviceA, deviceB]));
        await HausApiHandler.SetupPostAsJson($"{RoomsUrl}/{6}/add-devices", new { }, opts => opts.WithDelayMs(300));

        var page = Context.Render<DeviceDiscoveryView>();
        Eventually.Assert(() => Assert.Equal(2, page.FindAllByComponent<MudPaper>().Count()));

        var unassignedZone = page.FindByComponent<MudDropZone<DeviceModel>>(opts =>
            opts.WithText("unassigned devices")
        );
        var deviceAElement = unassignedZone.FindByTag(
            "div",
            opts => opts.WithClassName("device").WithText(deviceA.ExternalId)
        );
        await deviceAElement.DragStartAsync(new DragEventArgs());

        var bathroomZone = page.FindByComponent<MudDropZone<DeviceModel>>(opts => opts.WithText("bathroom"));
        var dropTask = bathroomZone.FindByTag("div").DropAsync(new DragEventArgs());

        Eventually.Assert(() =>
        {
            var deviceBElement = page.FindByTag(
                "div",
                opts => opts.WithClassName("mud-drop-item").WithText(deviceB.ExternalId)
            );
            Assert.Equal("false", deviceBElement.GetAttribute("draggable"));
        });

        await dropTask;

        Eventually.Assert(() =>
        {
            var deviceBElement = page.FindByTag(
                "div",
                opts => opts.WithClassName("mud-drop-item").WithText(deviceB.ExternalId)
            );
            Assert.Equal("true", deviceBElement.GetAttribute("draggable"));
        });
    }

    [Fact]
    public async Task WhenTwoDevicesAreDroppedInQuickSuccessionThenBothRetainRoomAssignment()
    {
        var light = HausModelFactory.DeviceModel() with { Id = 201, RoomId = null };
        var sensor = HausModelFactory.DeviceModel() with { Id = 202, RoomId = null };
        var room = HausModelFactory.RoomModel() with { Id = 6, Name = "bedroom" };

        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>([room]));
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>([light, sensor]));
        await HausApiHandler.SetupPostAsJson($"{RoomsUrl}/{room.Id}/add-devices", new { });

        var view = Context.Render<DeviceDiscoveryView>();
        Eventually.Assert(() => Assert.Equal(2, view.FindAllByComponent<MudPaper>().Count()));

        var container = view.FindComponent<MudDropContainer<DeviceModel>>();
        var lightDropTask = container.InvokeAsync(() =>
            container.Instance.ItemDropped.InvokeAsync(new MudItemDropInfo<DeviceModel>(light, room.Id.ToString(), 0))
        );
        var sensorDropTask = container.InvokeAsync(() =>
            container.Instance.ItemDropped.InvokeAsync(new MudItemDropInfo<DeviceModel>(sensor, room.Id.ToString(), 0))
        );
        await Task.WhenAll(lightDropTask, sensorDropTask);

        Eventually.Assert(() =>
        {
            var roomZone = view.FindByComponent<MudDropZone<DeviceModel>>(opts => opts.WithText(room.Name));
            Assert.Single(
                roomZone.FindAllByTag("div", opts => opts.WithClassName("device").WithText(light.ExternalId))
            );
            Assert.Single(
                roomZone.FindAllByTag("div", opts => opts.WithClassName("device").WithText(sensor.ExternalId))
            );
        });
    }
}
