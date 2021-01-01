using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
            var model = new RoomModel(Name: "Backroom");

            var result = await _bus.ExecuteCommandAsync(new CreateRoomCommand(model));

            var entity = _context.Set<RoomEntity>().Single();
            entity.Id.Should().Be(result.Id);
            entity.Name.Should().Be("Backroom");
        }

        [Fact]
        public async Task WhenModelIsInvalidThenThrowsValidationException()
        {
            var model = new RoomModel();

            Func<Task> act = () => _bus.ExecuteCommandAsync(new CreateRoomCommand(model));

            await act.Should().ThrowAsync<HausValidationException>();
        }
    }
}