using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Rooms;
using Haus.Site.Host.Rooms.AddRoom;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Rooms.AddRoom;

public class AddRoomDialogViewTests : HausSiteTestContext
{
    public const string RoomsUrl = "/api/rooms";

    [Fact]
    public async Task WhenOpenedThenShowsName()
    {
        var view = await RenderDialogAsync<AddRoomDialogView>();

        view.FindAllByComponent<MudTextField<string>>().Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenRoomIsSavedThenPostsRoomToApi()
    {
        var result = HausModelFactory.RoomModel();
        HttpRequestMessage? request = null;
        await HausApiHandler.SetupPostAsJson(RoomsUrl, result, opts => opts.WithCapture(r => request = r));

        var view = await RenderDialogAsync<AddRoomDialogView>();

        await view.InvokeAsync(async () =>
        {
            await view.FindMudTextFieldById<string>("name").Instance.SetText("Living Room");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        await Eventually.AssertAsync(async () =>
        {
            request.Should().NotBeNull();
            var room = request?.Content != null ? await request.Content.ReadFromJsonAsync<RoomModel>() : null;
            room?.Name.Should().Be("Living Room");
        });
    }

    [Fact]
    public async Task WhenAddIsCancelledThenDialogIsClosed()
    {
        var view = await RenderDialogAsync<AddRoomDialogView>();

        await view.InvokeAsync(async () =>
        {
            await view.FindMudButtonByText("cancel").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudDialog>().Should().HaveCount(0);
        });
    }

    [Fact]
    public async Task WhenRoomIsSavedThenClosesDialog()
    {
        var result = HausModelFactory.RoomModel();
        await HausApiHandler.SetupPostAsJson(RoomsUrl, result);

        var view = await RenderDialogAsync<AddRoomDialogView>();

        await view.InvokeAsync(async () =>
        {
            await view.FindMudTextFieldById<string>("name").Instance.SetText("Living Room");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            view.FindAllByComponent<MudDialog>().Should().HaveCount(0);
        });
    }

    [Fact]
    public async Task WhenSaveTakesAwhileThenButtonsAreDisabled()
    {
        var result = HausModelFactory.RoomModel();
        await HausApiHandler.SetupPostAsJson(RoomsUrl, result, opts => opts.WithDelayMs(500));
        var view = await RenderDialogAsync<AddRoomDialogView>();

        var invokeTask = view.InvokeAsync(async () =>
        {
            await view.FindMudTextFieldById<string>("name").Instance.SetText("Living Room");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            view.FindMudButtonByText("save").Instance.Disabled.Should().BeTrue();
            view.FindMudButtonByText("cancel").Instance.Disabled.Should().BeTrue();
            view.FindMudTextFieldById<string>("name").Instance.Disabled.Should().BeTrue();
        });
        await invokeTask;
    }
}
