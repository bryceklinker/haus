using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands;

public class UpdateRoomCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public UpdateRoomCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenRoomUpdatedThenRoomIsSavedToDatabase()
    {
        var original = _context.AddRoom();

        var command = new UpdateRoomCommand(new RoomModel(original.Id, "bob", 80));
        await _hausBus.ExecuteCommandAsync(command);

        var updated = await _context.FindByIdAsync<RoomEntity>(original.Id);
        Assert.Equal("bob", updated?.Name);
        Assert.Equal(80, updated?.OccupancyTimeoutInSeconds);
    }

    [Fact]
    public async Task WhenRoomUpdatedThenRoomUpdatedEventPublished()
    {
        var original = _context.AddRoom();

        var command = new UpdateRoomCommand(new RoomModel(original.Id, "bob"));
        await _hausBus.ExecuteCommandAsync(command);

        Assert.Single(_hausBus.GetPublishedRoutableEvents<RoomUpdatedEvent>());
    }

    [Fact]
    public async Task WhenRoomModelIsInvalidThenThrowsValidationException()
    {
        var original = _context.AddRoom();
        var command = new UpdateRoomCommand(new RoomModel(original.Id));

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await Assert.ThrowsAsync<HausValidationException>(act);
    }

    [Fact]
    public async Task WhenRoomIsMissingThenThrowsEntityNotFoundException()
    {
        var command = new UpdateRoomCommand(new RoomModel(54, "bob"));

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await Assert.ThrowsAsync<EntityNotFoundException<RoomEntity>>(act);
    }
}
