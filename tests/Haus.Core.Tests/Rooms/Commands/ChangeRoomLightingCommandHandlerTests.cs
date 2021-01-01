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
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands
{
    public class ChangeRoomLightingCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly CapturingHausBus _hausBus;

        public ChangeRoomLightingCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.CreateCapturingBus(_context);
        }

        [Fact]
        public async Task WhenLightingChangedThenRoomLightingIsSavedToDatabase()
        {
            var room = _context.AddRoom();

            var lighting = new LightingModel();
            await _hausBus.ExecuteCommandAsync(new ChangeRoomLightingCommand(room.Id, lighting));

            var updated = await _context.FindByIdAsync<RoomEntity>(room.Id);
            updated.Lighting.State.Should().Be(LightingState.Off);
        }

        [Fact]
        public async Task WhenLightingChangedThenRoomLightingChangedIsPublished()
        {
            var room = _context.AddRoom();
            var lighting = new LightingModel(LightingState.On);

            await _hausBus.ExecuteCommandAsync(new ChangeRoomLightingCommand(room.Id, lighting));

            var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
            hausCommand.Payload.Room.Id.Should().Be(room.Id);
            hausCommand.Payload.Lighting.State.Should().Be(LightingState.On);
        }

        [Fact]
        public async Task WhenRoomIsMissingThenThrowsEntityNotFoundException()
        {
            var command = new ChangeRoomLightingCommand(2423, new LightingModel());

            Func<Task> act = () => _hausBus.ExecuteCommandAsync(command);

            await act.Should().ThrowAsync<EntityNotFoundException<RoomEntity>>();
        }
    }
}