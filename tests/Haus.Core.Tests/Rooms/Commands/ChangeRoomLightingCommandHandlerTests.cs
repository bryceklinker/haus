using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
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

            var lighting = new LightingModel {State = LightingState.Off};
            await _hausBus.ExecuteCommandAsync(new ChangeRoomLightingCommand(room.Id, lighting));

            var updated = await _context.FindByIdAsync<RoomEntity>(room.Id);
            Assert.Equal(LightingState.Off, updated.Lighting.State);
        }

        [Fact]
        public async Task WhenLightingChangedThenRoomLightingChangedIsPublished()
        {
            var room = _context.AddRoom();
            var lighting = new LightingModel{State = LightingState.On};

            await _hausBus.ExecuteCommandAsync(new ChangeRoomLightingCommand(room.Id, lighting));

            var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
            Assert.Equal(room.Id, hausCommand.Payload.Room.Id);
            Assert.Equal(LightingState.On, hausCommand.Payload.Lighting.State);
        }

        [Fact]
        public async Task WhenRoomIsMissingThenThrowsEntityNotFoundException()
        {
            var command = new ChangeRoomLightingCommand(2423, new LightingModel());
            await Assert.ThrowsAsync<EntityNotFoundException<RoomEntity>>(() => _hausBus.ExecuteCommandAsync(command));
        }
    }
}