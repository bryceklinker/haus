using System.Linq;
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

        Assert.Equal(1, view.FindAllByComponent<MudTextField<string>>().Count());
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
            await view.FindMudTextFieldById<string>("name").Instance.SetTextAsync("Living Room");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        await Eventually.AssertAsync(async () =>
        {
            Assert.NotNull(request);
            var room = request?.Content != null ? await request.Content.ReadFromJsonAsync<RoomModel>() : null;
            Assert.Equal("Living Room", room?.Name);
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
            Assert.Empty(view.FindAllByComponent<MudDialog>());
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
            await view.FindMudTextFieldById<string>("name").Instance.SetTextAsync("Living Room");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            Assert.Empty(view.FindAllByComponent<MudDialog>());
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
            await view.FindMudTextFieldById<string>("name").Instance.SetTextAsync("Living Room");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        Eventually.Assert(() =>
        {
            Assert.True(view.FindMudButtonByText("save").Instance.Disabled);
            Assert.True(view.FindMudButtonByText("cancel").Instance.Disabled);
            Assert.True(view.FindMudTextFieldById<string>("name").Instance.Disabled);
        });
        await invokeTask;
    }
}
