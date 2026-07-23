using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.DomainEvents;

public class DeviceLightingChangedDomainEventHandlerTests
{
    private readonly CapturingHausBus _hausBus = HausBusFactory.CreateCapturingBus();

    [Fact]
    public async Task WhenDeviceLightingChangedThenRoutableCommandIsPublished()
    {
        var device = new DeviceEntity { Id = 123 };
        var lighting = new LightingEntity(Level: new LevelLightingEntity(34.12));
        _hausBus.Enqueue(new DeviceLightingChangedDomainEvent(device, lighting));

        await _hausBus.FlushAsync();

        var hausCommand = _hausBus.GetPublishedHausCommands<DeviceLightingChangedEvent>().Single();
        Assert.Equal(123, hausCommand.Payload?.Device.Id);
        Assert.Equal(new LevelLightingModel(34.12), hausCommand.Payload?.Lighting?.Level);
    }

    [Fact]
    public async Task WhenDeviceLightingChangedThenRoutableEventPublished()
    {
        var device = new DeviceEntity { Id = 123 };
        var lighting = new LightingEntity(Level: new LevelLightingEntity());
        _hausBus.Enqueue(new DeviceLightingChangedDomainEvent(device, lighting));

        await _hausBus.FlushAsync();

        Assert.Single(_hausBus.GetPublishedRoutableEvents<DeviceLightingChangedEvent>());
    }
}
