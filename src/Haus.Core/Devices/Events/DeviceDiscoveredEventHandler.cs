using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices.Discovery;

namespace Haus.Core.Devices.Events
{
    public class DeviceDiscoveredEventHandler : IEventHandler<RoutableEvent<DeviceDiscoveredModel>>
    {
        private readonly HausDbContext _context;

        public DeviceDiscoveredEventHandler(HausDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RoutableEvent<DeviceDiscoveredModel> notification, CancellationToken cancellationToken = default)
        {
            var existing = await _context.FindByAsync<DeviceEntity>(d => d.ExternalId == notification.Payload.Id);
            if (existing == null)
                _context.Add(DeviceEntity.FromDiscoveredDevice(notification.Payload));
            else
                existing.UpdateFromDiscoveredDevice(notification.Payload);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}