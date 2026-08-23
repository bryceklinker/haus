using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Events;

namespace Haus.Core.Zigbee.Events;

internal class ZigbeeDeviceJoinedEventHandler(IZigbeeStore store, IClock clock)
    : IEventHandler<RoutableEvent<ZigbeeDeviceJoinedEvent>>
{
    public Task Handle(RoutableEvent<ZigbeeDeviceJoinedEvent> notification, CancellationToken cancellationToken)
    {
        var payload = notification.Payload;
        var seenAt = clock.UtcNowOffset;

        store.PublishNext(s =>
            s.RecordDeviceJoined(payload.IeeeAddress, payload.NetworkAddress, seenAt)
                .RecordActivity(new ZigbeeActivityEntryModel(notification.Type, seenAt, payload))
        );
        return Task.CompletedTask;
    }
}
