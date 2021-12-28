using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Entities;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events
{
    internal class DeviceDiscoveredEventHandler : IEventHandler<RoutableEvent<DeviceDiscoveredEvent>>
    {
        private readonly IDeviceCommandRepository _repository;
        private readonly IHausBus _hausBus;

        public DeviceDiscoveredEventHandler(IHausBus hausBus, IDeviceCommandRepository repository)
        {
            _hausBus = hausBus;
            _repository = repository;
        }

        public async Task Handle(RoutableEvent<DeviceDiscoveredEvent> notification,
            CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByExternalId(notification.Payload.Id, cancellationToken).ConfigureAwait(false);
            if (existing == null)
                await CreateDeviceAsync(notification.Payload, cancellationToken).ConfigureAwait(false);
            else
                await UpdateDeviceAsync(existing, notification.Payload, cancellationToken).ConfigureAwait(false);
        }

        private async Task CreateDeviceAsync(DeviceDiscoveredEvent @event, CancellationToken cancellationToken)
        {
            var device = await _repository.AddAsync(DeviceEntity.FromDiscoveredDevice(@event, _hausBus), cancellationToken)
                .ConfigureAwait(false);

            await _hausBus.PublishAsync(RoutableEvent.FromEvent(new DeviceCreatedEvent(device.ToModel())), cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task UpdateDeviceAsync(DeviceEntity device, DeviceDiscoveredEvent @event, CancellationToken cancellationToken)
        {
            device.UpdateFromDiscoveredDevice(@event, _hausBus);
            await _repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
            await _hausBus.PublishAsync(RoutableEvent.FromEvent(new DeviceUpdatedEvent(device.ToModel())), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}