using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Entities;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events;

public class OccupancyChangedEventHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;
    private readonly DeviceEntity _sensor;
    private readonly RoomEntity _room;

    public OccupancyChangedEventHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);

        _sensor = _context.AddDevice(deviceType: DeviceType.MotionSensor);
        var light = _context.AddDevice(deviceType: DeviceType.Light);
        _room = _context.AddRoom(configure: entity =>
        {
            entity.AddDevice(_sensor, _hausBus);
            entity.AddDevice(light, _hausBus);
        });
    }

    [Fact]
    public async Task WhenOccupancySensorDetectsMotionAndSensorIsInARoomThenRoomLightsAreTurnedOn()
    {
        var change = new OccupancyChangedModel(_sensor.ExternalId, true);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
        hausCommand.Payload?.Room.Id.Should().Be(_room.Id);
        hausCommand.Payload?.Lighting.State.Should().Be(LightingState.On);
    }

    [Fact]
    public async Task WhenOccupancySensorDetectsMotionAndSensorIsInARoomThenRoomStateIsUpdatedToOn()
    {
        var change = new OccupancyChangedModel(_sensor.ExternalId, true);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        _context.ChangeTracker.HasChanges().Should().BeFalse();
    }

    [Fact]
    public async Task WhenOccupancySensorTimedOutAndSensorIsInRoomThenRoomLightsAreTurnedOff()
    {
        var change = new OccupancyChangedModel(_sensor.ExternalId);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
        hausCommand.Payload?.Room.Id.Should().Be(_room.Id);
        hausCommand.Payload?.Lighting.State.Should().Be(LightingState.Off);
    }

    [Fact]
    public async Task WhenOccupancySensorIsNotInARoomThenNoEventsArePublished()
    {
        var sensor = _context.AddDevice(deviceType: DeviceType.MotionSensor);

        var change = new OccupancyChangedModel(sensor.ExternalId, true);
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Should().BeEmpty();
    }
}
