using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Sensors;
using Haus.Cqrs;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events;

internal class MultiSensorChangedEventHandler(IHausBus hausBus) : IEventHandler<RoutableEvent<MultiSensorChanged>>
{
    public Task Handle(RoutableEvent<MultiSensorChanged> notification, CancellationToken cancellationToken)
    {
        return Task.WhenAll(GetPublishTasks(notification.Payload, cancellationToken));
    }

    private IEnumerable<Task> GetPublishTasks(MultiSensorChanged change, CancellationToken token)
    {
        if (change.OccupancyChanged != null)
        {
            yield return hausBus.PublishAsync(RoutableEvent.FromEvent(change.OccupancyChanged), token);
        }
    }
}
