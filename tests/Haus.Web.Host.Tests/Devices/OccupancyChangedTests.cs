using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class OccupancyChangedTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenOccupancyChangedForDeviceInRoomThenRoomLightingChangedPublished()
    {
        var (room, sensor) = await factory.AddRoomWithDevice("home", DeviceType.MotionSensor);

        var lightingCommands = new ConcurrentBag<HausCommand<RoomLightingChangedEvent>>();
        await factory.SubscribeToRoomLightingChangedCommandsAsync(lightingCommands.Add);
        await factory.PublishHausEventAsync(new OccupancyChangedModel(sensor.ExternalId, true));

        Eventually.Assert(() =>
        {
            lightingCommands
                .Where(cmd => cmd.Payload != null)
                .Should()
                .Contain(cmd => cmd.Payload!.Room.Id == room.Id && cmd.Payload.Lighting.State == LightingState.On);
        });
    }
}
