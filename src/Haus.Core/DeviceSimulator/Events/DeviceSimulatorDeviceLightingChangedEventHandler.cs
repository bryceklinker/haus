using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs.Events;

namespace Haus.Core.DeviceSimulator.Events;

internal class
    DeviceSimulatorDeviceLightingChangedEventHandler(IDeviceSimulatorStore store)
    : IEventHandler<RoutableEvent<DeviceLightingChangedEvent>>
{
    public Task Handle(RoutableEvent<DeviceLightingChangedEvent> notification, CancellationToken cancellationToken)
    {
        var deviceId = notification.Payload.Device.ExternalId;
        var lighting = notification.Payload.Lighting;
        store.PublishNext(s => s.ChangeDeviceLighting(deviceId, lighting));
        return Task.CompletedTask;
    }
}