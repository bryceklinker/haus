using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands;

public class TurnRoomOffCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public TurnRoomOffCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenRoomTurnedOffThenRoomLightingChangedCommandPublished()
    {
        var room = _context.AddRoom();

        await _hausBus.ExecuteCommandAsync(new TurnRoomOffCommand(room.Id));

        var publishedCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
        Assert.Equal(room.Id, publishedCommand.Payload?.Room.Id);
        Assert.Equal(LightingState.Off, publishedCommand.Payload?.Lighting.State);
    }

    [Fact]
    public async Task WhenRoomIsMissingThenThrowsNotFoundException()
    {
        var command = new TurnRoomOffCommand(234);

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await Assert.ThrowsAsync<EntityNotFoundException<RoomEntity>>(act);
    }
}
