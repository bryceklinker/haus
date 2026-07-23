using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.DomainEvents;
using Haus.Core.Rooms.Entities;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.DomainEvents;

public class RoomLightingChangedDomainEventHandlerTests
{
    private readonly CapturingHausBus _hausBus = HausBusFactory.CreateCapturingBus();

    [Fact]
    public async Task WhenRoomLightingChangedThenRoutableCommandIsPublished()
    {
        var room = new RoomEntity { Id = 89 };
        var lighting = new LightingEntity { State = LightingState.On };
        _hausBus.Enqueue(new RoomLightingChangedDomainEvent(room, lighting));

        await _hausBus.FlushAsync();

        var hausCommand = _hausBus.GetPublishedHausCommands<RoomLightingChangedEvent>().Single();
        Assert.Equal(89, hausCommand.Payload?.Room.Id);
        Assert.Equal(LightingState.On, hausCommand.Payload?.Lighting.State);
    }

    [Fact]
    public async Task WhenRoomLightingChangedThenRoutableEventIsPublished()
    {
        var room = new RoomEntity { Id = 89 };
        var lighting = new LightingEntity { State = LightingState.On };
        _hausBus.Enqueue(new RoomLightingChangedDomainEvent(room, lighting));

        await _hausBus.FlushAsync();

        Assert.Single(_hausBus.GetPublishedRoutableEvents<RoomLightingChangedEvent>());
    }
}
