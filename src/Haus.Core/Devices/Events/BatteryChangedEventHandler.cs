using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events;

internal class BatteryChangedEventHandler(IDeviceCommandRepository repository)
    : IEventHandler<RoutableEvent<BatteryChangedModel>>
{
    public async Task Handle(RoutableEvent<BatteryChangedModel> notification, CancellationToken cancellationToken)
    {
        var device = await repository
            .GetByExternalId(notification.Payload.DeviceId, cancellationToken)
            .ConfigureAwait(false);
        if (device == null)
            return;

        device.ChangeBatteryLevel(notification.Payload);

        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
    }
}
