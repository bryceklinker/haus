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

        page.FindAllByComponent<MudProgressCircular>().Should().HaveCount(1);
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
            page.FindAllByComponent<MudListItem<RoomModel>>().Should().HaveCount(3);
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
            page.FindByComponent<MudListItem<RoomModel>>().Instance.Text.Should().Contain("Living Room");
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
            var navigation = Context.Services.GetRequiredService<FakeNavigationManager>();
            navigation.Uri.Should().EndWith("/rooms/78");
        });
    }

    [Fact]
    public async Task WhenAddRoomStartedThenShowsAddRoomDialog()
    {
        await HausApiHandler.SetupGetAsJson(RoomsUrl, new ListResult<RoomModel>());

        var view = RenderView<RoomsListView>();
        await view.InvokeAsync(async () =>
        {
            await view.FindByComponent<MudFab>().Instance.OnClick.InvokeAsync(new MouseEventArgs());
        });

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudDialogContainer>().Should().HaveCount(1);
        });
    }
}
