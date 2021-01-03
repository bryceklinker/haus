using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices.Discovery;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events
{
    internal class DeviceDiscoveredEventHandler : IEventHandler<RoutableEvent<DeviceDiscoveredModel>>
    {
        private readonly HausDbContext _context;

        public DeviceDiscoveredEventHandler(HausDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RoutableEvent<DeviceDiscoveredModel> notification, CancellationToken cancellationToken = default)
        {
            var existing = await _context.FindByAsync<DeviceEntity>(d => d.ExternalId == notification.Payload.Id, token: cancellationToken);
            if (existing == null)
                _context.Add(DeviceEntity.FromDiscoveredDevice(notification.Payload));
            else
                existing.UpdateFromDiscoveredDevice(notification.Payload);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}