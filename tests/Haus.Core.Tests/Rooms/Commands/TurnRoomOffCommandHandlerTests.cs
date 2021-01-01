using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands
{
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
        public async Task WhenRoomTurnedOffThenRoomLightingEventPublished()
        {
            var room = _context.AddRoom();

            await _hausBus.ExecuteCommandAsync(new TurnRoomOffCommand(room.Id));

            var publishedCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
            publishedCommand.Payload.Room.Id.Should().Be(room.Id);
            publishedCommand.Payload.Lighting.State.Should().Be(LightingState.Off);
        }

        [Fact]
        public async Task WhenRoomIsMissingThenThrowsNotFoundException()
        {
            var command = new TurnRoomOffCommand(234);

            Func<Task> act = () => _hausBus.ExecuteCommandAsync(command);

            await act.Should().ThrowAsync<EntityNotFoundException<RoomEntity>>();
        }
    }
}