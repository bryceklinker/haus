using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Events;

namespace Haus.Core.Zigbee.Events;

internal class ZigbeeConnectionStatusChangedEventHandler(IZigbeeStore store, IClock clock)
    : IEventHandler<RoutableEvent<ZigbeeConnectionStatusChangedEvent>>
{
    public Task Handle(
        RoutableEvent<ZigbeeConnectionStatusChangedEvent> notification,
        CancellationToken cancellationToken
    )
    {
        var payload = notification.Payload;
        var status = new ZigbeeConnectionStatusModel(payload.IsConnected, payload.Reason, clock.UtcNowOffset);

        store.PublishNext(s =>
            s.UpdateConnectionStatus(status)
                .RecordActivity(new ZigbeeActivityEntryModel(notification.Type, clock.UtcNowOffset, payload))
        );
        return Task.CompletedTask;
    }
}
