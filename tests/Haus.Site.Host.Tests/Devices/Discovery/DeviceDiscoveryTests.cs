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
    public async Task WhenRenderedThenShowsAllRooms()
    {
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>([
            new RoomModel(Name: "Basement"),
            new RoomModel(Name: "Master Bedroom"),
        ]));

        var page = Context.RenderComponent<DeviceDiscovery>();

        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudListItem<RoomModel>>().Should().HaveCount(2);
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsAllUnassignedDevices()
    {
        await HausApiHandler.SetupGetAsJson(DevicesUrl, new ListResult<DeviceModel>([
            new DeviceModel(RoomId: 5),
            new DeviceModel(RoomId: null),
            new DeviceModel(RoomId: null),
            new DeviceModel(RoomId: 6),
        ]));
        
        var page = Context.RenderComponent<DeviceDiscovery>();
        
        Eventually.Assert(() =>
        {
            page.FindAllByComponent<MudListItem<DeviceModel>>().Should().HaveCount(2);
        });
    }
}