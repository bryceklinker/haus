using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Core.Zigbee.State;
using Haus.Cqrs.Events;

namespace Haus.Core.Zigbee.Events;

internal class ZigbeeTransportErrorEventHandler(IZigbeeStore store, IClock clock)
    : IEventHandler<RoutableEvent<ZigbeeTransportErrorEvent>>
{
    public Task Handle(RoutableEvent<ZigbeeTransportErrorEvent> notification, CancellationToken cancellationToken)
    {
        store.PublishNext(s =>
            s.RecordActivity(new ZigbeeActivityEntryModel(notification.Type, clock.UtcNowOffset, notification.Payload))
        );
        return Task.CompletedTask;
    }
}
