using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands
{
    public class CreateRoomCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly CapturingHausBus _bus;

        public CreateRoomCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _bus = HausBusFactory.CreateCapturingBus(_context);
        }

        [Fact]
        public async Task WhenRoomCreatedThenReturnsModelWithRoomId()
        {
            var model = new RoomModel(Name: "Backroom", OccupancyTimeoutInSeconds: 70);

            var result = await _bus.ExecuteCommandAsync(new CreateRoomCommand(model));

            var entity = _context.Set<RoomEntity>().Single();
            entity.Id.Should().Be(result.Id);
            entity.Name.Should().Be("Backroom");
            entity.OccupancyTimeoutInSeconds.Should().Be(70);
        }

        [Fact]
        public async Task WhenRoomCreatedThenPublishesRoomCreatedEvent()
        {
            var model = new RoomModel(Name: $"{Guid.NewGuid()}");

            await _bus.ExecuteCommandAsync(new CreateRoomCommand(model));

            _bus.GetPublishedRoutableEvents<RoomCreatedEvent>().Should().HaveCount(1);
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