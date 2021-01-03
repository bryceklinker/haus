using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class MultiSensorChangedTests
    {
        private readonly HausWebHostApplicationFactory _factory;

        public MultiSensorChangedTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenMultiSensorChangedWithOccupancyThenRoomLightingChangedPublished()
        {
            const DeviceType multiSensorDeviceType =
                DeviceType.MotionSensor | DeviceType.LightSensor | DeviceType.TemperatureSensor;
            var (room, sensor) = await _factory.AddRoomWithDevice("sup", multiSensorDeviceType);

            HausCommand<RoomLightingChangedEvent> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(cmd => hausCommand = cmd);
            await _factory.PublishHausEventAsync(new MultiSensorChanged(
                sensor.ExternalId,
                new OccupancyChangedModel(sensor.ExternalId, true)
            ));

            Eventually.Assert(() =>
            {
                hausCommand.Payload.Room.Id.Should().Be(room.Id);
                hausCommand.Payload.Lighting.State.Should().Be(LightingState.On);
            });
        }
    }
}