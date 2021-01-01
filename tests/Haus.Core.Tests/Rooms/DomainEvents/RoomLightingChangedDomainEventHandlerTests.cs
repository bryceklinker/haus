using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Models.Common;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.DomainEvents;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.DomainEvents
{
    public class RoomLightingChangedDomainEventHandlerTests
    {
        private readonly CapturingHausBus _hausBus;

        public RoomLightingChangedDomainEventHandlerTests()
        {
            _hausBus = HausBusFactory.CreateCapturingBus();
        }
        
        [Fact]
        public async Task WhenRoomLightingChangedThenRoutableCommandIsPublished()
        {
            var room = new RoomEntity{Id = 89};
            var lighting = new Lighting{State = LightingState.On};
            _hausBus.Enqueue(new RoomLightingChangedDomainEvent(room, lighting));

            await _hausBus.FlushAsync();

            var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
            hausCommand.Payload.Room.Id.Should().Be(89);
            hausCommand.Payload.Lighting.State.Should().Be(LightingState.On);
        }
    }
}