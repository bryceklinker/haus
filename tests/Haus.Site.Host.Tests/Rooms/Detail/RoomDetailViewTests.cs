using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Site.Host.Rooms.Detail;
using Haus.Site.Host.Shared.Lighting;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;

namespace Haus.Site.Host.Tests.Rooms.Detail;

public class RoomDetailViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsRoomInformation()
    {
        var room = HausModelFactory.RoomModel();

        var view = RenderDetail(room);

        view.FindMudTextFieldById<string?>("name").Instance.Value.Should().Be(room.Name);
        view.FindMudTextFieldById<int?>("occupancyTimeout").Instance.Value.Should().Be(room.OccupancyTimeoutInSeconds);
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
            await view.FindMudTextFieldById<string?>("name").Instance.SetText("bill");
            await view.FindMudTextFieldById<int?>("occupancyTimeout").Instance.SetText("500");
            await view.FindMudButtonByText("save").ClickAsync();
        });

        await Eventually.AssertAsync(async () =>
        {
            var model = request?.Content != null ? await request.Content.ReadFromJsonAsync<RoomModel>() : null;
            model?.OccupancyTimeoutInSeconds.Should().Be(500);
            model?.Name.Should().Be("bill");
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

            newLighting.Should().BeEquivalentTo(lighting);
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
