using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Cqrs.DomainEvents;

namespace Haus.Core.Devices.DomainEvents;

public record DeviceLightingChangedDomainEvent(DeviceEntity Device, LightingEntity Lighting) : IDomainEvent
{
    public DeviceModel DeviceModel => Device.ToModel();
    public LightingModel LightingModel => Lighting.ToModel();
}

internal class DeviceLightingChangedDomainEventHandler(IHausBus hausBus)
    : IDomainEventHandler<DeviceLightingChangedDomainEvent>
{
    public Task Handle(DeviceLightingChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = new DeviceLightingChangedEvent(notification.DeviceModel, notification.LightingModel);
        return Task.WhenAll(
            hausBus.PublishAsync(RoutableCommand.FromEvent(@event), cancellationToken),
            hausBus.PublishAsync(RoutableEvent.FromEvent(@event), cancellationToken)
        );
    }
}
