using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events;

internal class TemperatureChangedEventHandler(IDeviceCommandRepository repository)
    : IEventHandler<RoutableEvent<TemperatureChangedModel>>
{
    public async Task Handle(RoutableEvent<TemperatureChangedModel> notification, CancellationToken cancellationToken)
    {
        var device = await repository
            .GetByExternalId(notification.Payload.DeviceId, cancellationToken)
            .ConfigureAwait(false);
        if (device == null)
            return;

        device.ChangeTemperature(notification.Payload);

        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
    }
}
