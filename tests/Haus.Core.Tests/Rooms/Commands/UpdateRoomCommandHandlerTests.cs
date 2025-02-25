using System.Threading.Tasks;
using FluentAssertions;
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
        updated.Name.Should().Be("bob");
        updated.OccupancyTimeoutInSeconds.Should().Be(80);
    }

    [Fact]
    public async Task WhenRoomUpdatedThenRoomUpdatedEventPublished()
    {
        var original = _context.AddRoom();

        var command = new UpdateRoomCommand(new RoomModel(original.Id, "bob"));
        await _hausBus.ExecuteCommandAsync(command);

        _hausBus.GetPublishedRoutableEvents<RoomUpdatedEvent>().Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenRoomModelIsInvalidThenThrowsValidationException()
    {
        var original = _context.AddRoom();
        var command = new UpdateRoomCommand(new RoomModel(original.Id));

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<HausValidationException>();
    }

    [Fact]
    public async Task WhenRoomIsMissingThenThrowsEntityNotFoundException()
    {
        var command = new UpdateRoomCommand(new RoomModel(54, "bob"));

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<EntityNotFoundException<RoomEntity>>();
    }
}
