using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Sensors;

namespace Haus.Core.Devices.Events
{
    internal class MultiSensorChangedEventHandler : IEventHandler<RoutableEvent<MultiSensorChanged>>
    {
        private readonly IHausBus _hausBus;

        public MultiSensorChangedEventHandler(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }

        public Task Handle(RoutableEvent<MultiSensorChanged> notification, CancellationToken cancellationToken)
        {
            return Task.WhenAll(GetPublishTasks(notification.Payload, cancellationToken));
        }

        private IEnumerable<Task> GetPublishTasks(MultiSensorChanged change, CancellationToken token)
        {
            if (change.HasOccupancy)
                yield return _hausBus.PublishAsync(RoutableEvent.FromEvent(change.OccupancyChanged), token);
        }
    }
}