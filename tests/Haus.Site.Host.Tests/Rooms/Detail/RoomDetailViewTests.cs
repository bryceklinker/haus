using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Site.Host.Rooms.Detail;
using Haus.Site.Host.Shared.Lighting;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;
using MudBlazor.Extensions;

namespace Haus.Site.Host.Tests.Rooms.Detail;

public class RoomDetailViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsRoomInformation()
    {
        var room = HausModelFactory.RoomModel();

        var view = RenderDetail(room);

        Assert.Equal(room.Name, view.FindMudTextFieldById<string?>("name").Instance.GetState(x => x.Value));
        Assert.Equal(
            room.OccupancyTimeoutInSeconds,
            view.FindMudTextFieldById<int?>("occupancyTimeout").Instance.GetState(x => x.Value)
        );
    }

    [Fact]
    public async Task WhenRoomIsModifiedAndSavedThenUpdatesRoom()
    {
        HttpRequestMessage? request = null;
        await HausApiHandler.SetupPutAsJson("/api/rooms/90", new { }, opts => opts.WithCapture(r => request = r));

        var room = HausModelFactory.RoomModel() with { Id = 90 };

        var view = RenderDetail(room);

        await view.InvokeAsync(async () =>
        {
            await view.FindMudTextFieldById<string?>("name").Instance.SetTextAsync("bill");
            await view.FindMudTextFieldById<int?>("occupancyTimeout").Instance.SetTextAsync("500");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        await Eventually.AssertAsync(async () =>
        {
            var model = request?.Content != null ? await request.Content.ReadFromJsonAsync<RoomModel>() : null;
            Assert.Equal(500, model?.OccupancyTimeoutInSeconds);
            Assert.Equal("bill", model?.Name);
        });
    }

    [Fact]
    public async Task WhenLightingIsChangedThenRoomLightingIsUpdated()
    {
        HttpRequestMessage? lightingRequest = null;
        await HausApiHandler.SetupPutAsJson(
            "/api/rooms/8/lighting",
            new { },
            opts => opts.WithCapture(r => lightingRequest = r)
        );
        var room = HausModelFactory.RoomModel() with { Id = 8 };
        await HausApiHandler.SetupGetAsJson("/api/rooms/8", room);

        var view = RenderDetail(room);

        var lighting = HausModelFactory.LightingModel();
        await view.InvokeAsync(async () =>
        {
            await view.FindByComponent<LightingView>().Instance.OnLightingChanged.InvokeAsync(lighting);
        });

        await Eventually.AssertAsync(async () =>
        {
            var newLighting =
                lightingRequest?.Content == null
                    ? null
                    : await lightingRequest.Content.ReadFromJsonAsync<LightingModel>();

            Assert.Equal(lighting, newLighting);
        });
    }

    private IRenderedComponent<RoomDetailView> RenderDetail(RoomModel room)
    {
        return RenderView<RoomDetailView>(opts =>
        {
            opts.Add(o => o.Room, room);
        });
    }
}
