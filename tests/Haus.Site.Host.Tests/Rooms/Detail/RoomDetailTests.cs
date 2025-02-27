using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Lighting;
using Haus.Site.Host.Rooms.Detail;
using Haus.Site.Host.Shared.Lighting;
using Haus.Site.Host.Tests.Support;
using Haus.Testing.Support;

namespace Haus.Site.Host.Tests.Rooms.Detail;

public class RoomDetailTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsRoomInformation()
    {
        var room = HausModelFactory.RoomModel();

        var page = Context.RenderComponent<RoomDetail>(opts =>
        {
            opts.Add(o => o.Room, room);
        });

        page.FindMudTextFieldById<string>("name").Instance.Value.Should().Be(room.Name);
        page.FindMudTextFieldById<int>("occupancyTimeout").Instance.Value.Should().Be(room.OccupancyTimeoutInSeconds);
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

        var page = Context.RenderComponent<RoomDetail>(opts =>
        {
            opts.Add(o => o.Room, room);
        });

        var lighting = HausModelFactory.LightingModel();
        await page.InvokeAsync(async () =>
        {
            await page.FindByComponent<LightingView>().Instance.OnLightingChanged.InvokeAsync(lighting);
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

    [Fact]
    public async Task WhenLightingTakesAWhileToUpdateThenLightingIsDisabled()
    {
        await HausApiHandler.SetupPutAsJson("/api/rooms/8/lighting", new { }, opts => opts.WithDelayMs(500));

        var room = HausModelFactory.RoomModel() with { Id = 8 };
        var page = Context.RenderComponent<RoomDetail>(opts =>
        {
            opts.Add(o => o.Room, room);
        });
        var lighting = HausModelFactory.LightingModel();
        await page.InvokeAsync(async () =>
        {
            await page.FindByComponent<LightingView>().Instance.OnLightingChanged.InvokeAsync(lighting);
        });

        Eventually.Assert(() =>
        {
            page.FindByComponent<LightingView>().Instance.Disabled.Should().BeTrue();
        });
    }
}
