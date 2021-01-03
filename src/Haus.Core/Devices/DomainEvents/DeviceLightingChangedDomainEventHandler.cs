using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs;
using Haus.Cqrs.DomainEvents;

namespace Haus.Core.Devices.DomainEvents
{
    public record DeviceLightingChangedDomainEvent(DeviceEntity Device, LightingEntity Lighting) : IDomainEvent
    {
        public DeviceModel DeviceModel => Device.ToModel();
        public LightingModel LightingModel => Lighting.ToModel();
    }
    
    internal class DeviceLightingChangedDomainEventHandler : IDomainEventHandler<DeviceLightingChangedDomainEvent>
    {
        private readonly IHausBus _hausBus;

        public DeviceLightingChangedDomainEventHandler(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }

        public Task Handle(DeviceLightingChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var @event = new DeviceLightingChangedEvent(notification.DeviceModel, notification.LightingModel);
            return _hausBus.PublishAsync(RoutableCommand.FromEvent(@event), cancellationToken);
        }
    }
}