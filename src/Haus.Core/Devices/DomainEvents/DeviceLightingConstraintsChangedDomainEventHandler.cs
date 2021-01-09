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

namespace Haus.Core.Devices.DomainEvents
{
    public record DeviceLightingConstraintsChangedDomainEvent(
        DeviceEntity Device,
        LightingConstraintsEntity Constraints) : IDomainEvent
    {
        public DeviceModel DeviceModel => Device.ToModel();
        public LightingConstraintsModel ConstraintsModel => Constraints.ToModel();
    }

    internal class DeviceLightingConstraintsChangedDomainEventHandler : IDomainEventHandler<DeviceLightingConstraintsChangedDomainEvent>
    {
        private readonly IHausBus _hausBus;

        public DeviceLightingConstraintsChangedDomainEventHandler(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }


        public Task Handle(DeviceLightingConstraintsChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var device = notification.DeviceModel;
            var constraints = notification.ConstraintsModel;
            return _hausBus.PublishAsync(RoutableEvent.FromEvent(new DeviceLightingConstraintsChangedEvent(device, constraints)), cancellationToken);
        }
    }
}