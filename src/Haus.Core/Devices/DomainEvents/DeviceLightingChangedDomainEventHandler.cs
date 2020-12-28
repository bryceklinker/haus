using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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

        public DeviceLightingChangedDomainEvent(DeviceEntity device, Lighting lighting)
        {
            Device = device;
            Lighting = lighting;
        }
    }
    
    internal class DeviceLightingChangedDomainEventHandler : IDomainEventHandler<DeviceLightingChangedDomainEvent>
    {
        private readonly IHausBus _hausBus;
        private readonly IMapper _mapper;

        public DeviceLightingChangedDomainEventHandler(IHausBus hausBus, IMapper mapper)
        {
            _hausBus = hausBus;
            _mapper = mapper;
        }

        public Task Handle(DeviceLightingChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var device = _mapper.Map<DeviceModel>(notification.Device);
            var lighting = _mapper.Map<LightingModel>(notification.Lighting);

            var @event = new DeviceLightingChangedEvent(device, lighting);
            return _hausBus.PublishAsync(RoutableCommand.FromEvent(@event), cancellationToken);
        }
    }
}