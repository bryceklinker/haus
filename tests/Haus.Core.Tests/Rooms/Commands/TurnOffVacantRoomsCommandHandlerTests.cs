using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands;

public class TurnOffVacantRoomsCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _bus;

    public TurnOffVacantRoomsCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _bus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenTurnOffVacantRoomsExecutedAndRoomIsVacantThenChangesRoomLightingToOff()
    {
        var room = AddOccupiedRoom();
        
        await _bus.ExecuteCommandAsync(new TurnOffVacantRoomsCommand());

        var updatedRoom = await _context.FindByIdAsync<RoomEntity>(room.Id);
        updatedRoom.Lighting.State.Should().Be(LightingState.Off);
    }

    [Fact]
    public async Task WhenTurnOffVacantRoomsExecutedAndRoomIsVacantThenPublishesRoomLightingChange()
    {
        var room = AddOccupiedRoom();
        
        await _bus.ExecuteCommandAsync(new TurnOffVacantRoomsCommand());

        var lightingChange = _bus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
        lightingChange.Payload.Lighting.State.Should().Be(LightingState.Off);
        lightingChange.Payload.Room.Id.Should().Be(room.Id);
    }

    [Fact]
    public async Task WhenTurnOffVacantRoomsExecutedAndRoomIsOccupiedThenDoesNothing()
    {
        var room = AddOccupiedRoom(300);

        await _bus.ExecuteCommandAsync(new TurnOffVacantRoomsCommand());

        var updatedRoom = await _context.FindByIdAsync<RoomEntity>(room.Id);
        updatedRoom.Lighting.State.Should().Be(LightingState.On);
        _bus.GetPublishedHausCommands<RoomLightingChangedEvent>().Should().BeEmpty();
    }

    [Fact]
    public async Task WhenTurnOffVacantRoomsExecutedAndMultipleRoomsAreVacantThenEachRoomIsTurnedOff()
    {
        AddOccupiedRoom();
        AddOccupiedRoom();
        AddOccupiedRoom();

        await _bus.ExecuteCommandAsync(new TurnOffVacantRoomsCommand());

        _bus.GetPublishedHausCommands<RoomLightingChangedEvent>().Should().HaveCount(3);
    }

    [Fact]
    public async Task WhenRoomLightingIsOffThenRoomLightingIsNotChanged()
    {
        _context.AddRoom("off", r =>
        {
            r.ChangeLighting(new LightingEntity(), new FakeDomainEventBus());
            r.ChangeOccupancy(new OccupancyChangedModel("idk", true), new FakeDomainEventBus());
        });

        await _bus.ExecuteCommandAsync(new TurnOffVacantRoomsCommand());

        _bus.GetPublishedHausCommands<RoomLightingChangedEvent>().Should().BeEmpty();
    }

    [Fact]
    public async Task WhenRoomIsMissingLastOccupiedThenRoomLightingIsNotChanged()
    {
        _context.AddRoom("off", r => r.ChangeLighting(new LightingEntity(LightingState.On), _bus));

        await _bus.ExecuteCommandAsync(new TurnOffVacantRoomsCommand());

        _bus.GetPublishedHausCommands<RoomLightingChangedEvent>().Should().BeEmpty();
    }

    private RoomEntity AddOccupiedRoom(int occupancyTimeoutInSeconds = 0)
    {
        return _context.AddRoom("one",
            e =>
            {
                e.OccupancyTimeoutInSeconds = occupancyTimeoutInSeconds;
                e.ChangeOccupancy(new OccupancyChangedModel("", true), _bus);
            }
        );
    }
}