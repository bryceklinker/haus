using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Events;

namespace Haus.Core.Zigbee.Events;

internal class ZigbeeDeviceInfoDiscoveredEventHandler(IZigbeeStore store, IClock clock)
    : IEventHandler<RoutableEvent<ZigbeeDeviceInfoDiscoveredEvent>>
{
    public Task Handle(RoutableEvent<ZigbeeDeviceInfoDiscoveredEvent> notification, CancellationToken cancellationToken)
    {
        var payload = notification.Payload;
        var seenAt = clock.UtcNowOffset;

        store.PublishNext(s =>
            s.RecordDeviceInfoDiscovered(
                    payload.IeeeAddress,
                    payload.ManufacturerName,
                    payload.ModelIdentifier,
                    payload.Endpoints,
                    seenAt
                )
                .RecordActivity(new ZigbeeActivityEntryModel(notification.Type, seenAt, payload))
        );
        return Task.CompletedTask;
    }
}
