using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events
{
    public class OccupancyChangedEventHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly CapturingHausBus _hausBus;
        private DeviceEntity _sensor;
        private DeviceEntity _light;
        private RoomEntity _room;

        public OccupancyChangedEventHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.CreateCapturingBus(_context);
            
            _sensor = _context.AddDevice(deviceType: DeviceType.MotionSensor);
            _light = _context.AddDevice(deviceType: DeviceType.Light);
            _room = _context.AddRoom(configure: entity =>
            {
                entity.AddDevice(_sensor, _hausBus);
                entity.AddDevice(_light, _hausBus);
            });
        }

        [Fact]
        public async Task WhenOccupancySensorDetectsMotionAndSensorIsInARoomThenRoomLightsAreTurnedOn()
        {
            var change = new OccupancyChangedModel {Occupancy = true, DeviceId = _sensor.ExternalId};
            await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

            var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
            Assert.Equal(_room.Id, hausCommand.Payload.Room.Id);
            Assert.Equal(LightingState.On, hausCommand.Payload.Lighting.State);
        }

        [Fact]
        public async Task WhenOccupancySensorTimedOutAndSensorIsInRoomThenRoomLightsAreTurnedOff()
        {
            var change = new OccupancyChangedModel {Occupancy = false, DeviceId = _sensor.ExternalId};
            await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

            var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
            Assert.Equal(_room.Id, hausCommand.Payload.Room.Id);
            Assert.Equal(LightingState.Off, hausCommand.Payload.Lighting.State);
        }
        
        [Fact]
        public async Task WhenOccupancySensorIsNotInARoomThenNoEventsArePublished()
        {
            var sensor = _context.AddDevice(deviceType: DeviceType.MotionSensor);
            
            var change = new OccupancyChangedModel{Occupancy = true, DeviceId = sensor.ExternalId};
            await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));
            
            Assert.Empty(_hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>());
        }
    }
}