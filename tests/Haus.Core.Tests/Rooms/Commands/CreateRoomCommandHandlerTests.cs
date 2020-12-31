using System.Linq;
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
    public class CreateRoomCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _bus;

        public CreateRoomCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _bus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenRoomCreatedThenReturnsModelWithRoomId()
        {
            var model = new RoomModel(name: "Backroom");

            var result = await _bus.ExecuteCommandAsync(new CreateRoomCommand(model));

            var entity = _context.Set<RoomEntity>().Single();
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("Backroom", result.Name);
        }

        [Fact]
        public async Task WhenModelIsInvalidThenThrowsValidationException()
        {
            var model = new RoomModel();
            
            await Assert.ThrowsAsync<HausValidationException>(() => _bus.ExecuteCommandAsync(new CreateRoomCommand(model)));
        }
    }
}