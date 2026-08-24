using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Events;

namespace Haus.Core.Zigbee.Events;

internal class ZigbeeCommandDroppedEventHandler(IZigbeeStore store, IClock clock)
    : IEventHandler<RoutableEvent<ZigbeeCommandDroppedEvent>>
{
    public Task Handle(RoutableEvent<ZigbeeCommandDroppedEvent> notification, CancellationToken cancellationToken)
    {
        store.PublishNext(s =>
            s.RecordActivity(new ZigbeeActivityEntryModel(notification.Type, clock.UtcNowOffset, notification.Payload))
        );
        return Task.CompletedTask;
    }
}
