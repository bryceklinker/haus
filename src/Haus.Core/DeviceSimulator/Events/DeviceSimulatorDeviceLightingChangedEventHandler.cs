using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs.Events;

namespace Haus.Core.DeviceSimulator.Events
{
    internal class DeviceSimulatorDeviceLightingChangedEventHandler : IEventHandler<RoutableEvent<DeviceLightingChangedEvent>>
    {
        private readonly IDeviceSimulatorStore _store;

        public DeviceSimulatorDeviceLightingChangedEventHandler(IDeviceSimulatorStore store)
        {
            _store = store;
        }

        public Task Handle(RoutableEvent<DeviceLightingChangedEvent> notification, CancellationToken cancellationToken)
        {
            var deviceId = notification.Payload.Device.ExternalId;
            var lighting = notification.Payload.Lighting;
            _store.PublishNext(s => s.ChangeDeviceLighting(deviceId, lighting));
            return Task.CompletedTask;
        }
    }
}