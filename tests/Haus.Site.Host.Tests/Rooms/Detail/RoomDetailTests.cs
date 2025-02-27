using Haus.Site.Host.Rooms.Detail;
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
}
