using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class MultiSensorChangedTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenMultiSensorChangedWithOccupancyThenRoomLightingChangedPublished()
    {
        const DeviceType multiSensorDeviceType =
            DeviceType.MotionSensor | DeviceType.LightSensor | DeviceType.TemperatureSensor;
        var (room, sensor) = await factory.AddRoomWithDevice("sup", multiSensorDeviceType);

        var commands = new ConcurrentBag<HausCommand<RoomLightingChangedEvent>>();
        await factory.SubscribeToRoomLightingChangedCommandsAsync(commands.Add);
        await factory.PublishHausEventAsync(
            new MultiSensorChanged(sensor.ExternalId, new OccupancyChangedModel(sensor.ExternalId, true))
        );

        Eventually.Assert(() =>
        {
            commands
                .Where(cmd => cmd.Payload != null)
                .Should()
                .Contain(cmd => cmd.Payload!.Room.Id == room.Id && cmd.Payload.Lighting.State == LightingState.On);
        });
    }
}
