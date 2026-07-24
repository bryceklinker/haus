using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms;
using Haus.Site.Host.Rooms.AddRoom;
using Haus.Site.Host.Rooms.List;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Haus.Site.Host.Tests.Rooms.List;

public class RoomsListViewTest : HausSiteTestContext
{
    private const string RoomsUrl = "/api/rooms";

    [Fact]
    public async Task WhenRenderedThenShowsLoadingWhileRoomsAreLoaded()
    {
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>(), opts => opts.WithDelayMs(500));

        var page = RenderView<RoomsListView>();

        Assert.Equal(1, page.FindAllByComponent<MudProgressCircular>().Count());
    }

    [Fact]
    public async Task WhenRenderedThenShowsAListOfRooms()
    {
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>(
                [HausModelFactory.RoomModel(), HausModelFactory.RoomModel(), HausModelFactory.RoomModel()]
            )
        );

        var page = RenderView<RoomsListView>();

        Eventually.Assert(() =>
        {
            Assert.Equal(3, page.FindAllByComponent<MudListItem<RoomModel>>().Count());
        });
    }

    [Fact]
    public async Task WhenRenderedThenShowsTheRoomName()
    {
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>([HausModelFactory.RoomModel() with { Name = "Living Room" }])
        );

        var page = RenderView<RoomsListView>();

        Eventually.Assert(() =>
        {
            Assert.Contains("Living Room", page.FindByComponent<MudListItem<RoomModel>>().Instance.Text);
        });
    }

    [Fact]
    public async Task WhenRoomIsSelectedThenNavigatesToRoom()
    {
        await HausApiHandler.SetupGetAsJson(
            RoomsUrl,
            new ListResult<RoomModel>([HausModelFactory.RoomModel() with { Id = 78 }])
        );

        var view = RenderView<RoomsListView>();
        await view.InvokeAsync(async () =>
        {
            await view.FindByComponent<MudListItem<RoomModel>>().Instance.OnClick.InvokeAsync(new MouseEventArgs());
        });

        Eventually.Assert(() =>
        {
            var navigation = Context.Services.GetRequiredService<BunitNavigationManager>();
            Assert.EndsWith("/rooms/78", navigation.Uri);
        });
    }

    [Fact]
    public async Task WhenAddRoomStartedThenShowsAddRoomDialog()
    {
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>());

        var dialogProvider = Context.Render<MudDialogProvider>();
        var view = RenderView<RoomsListView>();
        await view.InvokeAsync(async () =>
        {
            await view.FindByComponent<MudFab>().Instance.OnClick.InvokeAsync(new MouseEventArgs());
        });

        Eventually.Assert(() =>
        {
            Assert.Equal(1, dialogProvider.FindAllByComponent<AddRoomDialogView>().Count());
        });
    }
}
