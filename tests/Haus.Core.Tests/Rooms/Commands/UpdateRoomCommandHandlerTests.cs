using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands
{
    public class UpdateRoomCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public UpdateRoomCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenRoomUpdatedThenRoomIsSavedToDatabase()
        {
            var original = _context.AddRoom();
            var command = new UpdateRoomCommand(new RoomModel(original.Id, "bob"));

            await _hausBus.ExecuteCommandAsync(command);

            var updated = await _context.FindByIdAsync<RoomEntity>(original.Id);
            Assert.Equal("bob", updated.Name);
        }

        [Fact]
        public async Task WhenRoomModelIsInvalidThenThrowsValidationException()
        {
            var original = _context.AddRoom();
            var command = new UpdateRoomCommand(new RoomModel(original.Id));

            await Assert.ThrowsAsync<HausValidationException>(() => _hausBus.ExecuteCommandAsync(command));
        }

        [Fact]
        public async Task WhenRoomIsMissingThenThrowsEntityNotFoundException()
        {
            var command = new UpdateRoomCommand(new RoomModel(54, Name: "bob"));

            await Assert.ThrowsAsync<EntityNotFoundException<RoomEntity>>(() => _hausBus.ExecuteCommandAsync(command));
        }
    }
}