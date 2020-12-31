using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs;
using Haus.Cqrs.DomainEvents;

namespace Haus.Core.Devices.DomainEvents
{
    public class DeviceLightingChangedDomainEvent : IDomainEvent
    {
        public DeviceEntity Device { get; }
        public Lighting Lighting { get; }

        public DeviceModel DeviceModel => Device.ToModel();
        public LightingModel LightingModel => Lighting.ToModel();
        
        public DeviceLightingChangedDomainEvent(DeviceEntity device, Lighting lighting)
        {
            Device = device;
            Lighting = lighting;
        }
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