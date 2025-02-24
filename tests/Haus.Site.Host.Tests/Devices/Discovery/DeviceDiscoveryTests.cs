using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Haus.Site.Host.Devices.Discovery;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Devices.Discovery;

public class DeviceDiscoveryTests : HausSiteTestContext
{
    private const string RoomsUrl = "/api/rooms";
    private const string DevicesUrl = "/api/devices";

    [Fact]
    public async Task WhenRenderedThenShowsLoading()
    {
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>(), opts => opts.WithDelayMs(1000));
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>(), opts => opts.WithDelayMs(1000));

        var page = Context.RenderComponent<DeviceDiscovery>();
        
        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
        });
    }
    
    [Fact]
    public async Task WhenRenderedThenShowsAllRooms()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>());
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>([
            HausModelFactory.RoomModel() with {Name = "Basement", Id = 4},
            HausModelFactory.RoomModel() with {Name = "Master Bedroom", Id = 3},
        ]));

        var page = Context.RenderComponent<DeviceDiscovery>();

        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudDropZone<DeviceModel>>().Should().HaveCount(3);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsAllUnassignedDevices()
    {
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>([
            HausModelFactory.RoomModel() with { Id = 6 }
        ]));
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>([
            HausModelFactory.DeviceModel() with {RoomId = 6},
            HausModelFactory.DeviceModel() with {RoomId = null},
            HausModelFactory.DeviceModel() with {RoomId = null},
            HausModelFactory.DeviceModel() with {RoomId = 6},
        ]));
        
        var page = Context.RenderComponent<DeviceDiscovery>();
        await Task.Delay(1000);
        
        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudPaper>().Should().HaveCount(4);
        });
    }
}