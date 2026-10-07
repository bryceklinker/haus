using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events;

internal class IlluminanceChangedEventHandler(IDeviceCommandRepository repository)
    : IEventHandler<RoutableEvent<IlluminanceChangedModel>>
{
    public async Task Handle(RoutableEvent<IlluminanceChangedModel> notification, CancellationToken cancellationToken)
    {
        var device = await repository
            .GetByExternalId(notification.Payload.DeviceId, cancellationToken)
            .ConfigureAwait(false);
        if (device == null)
            return;

        device.ChangeIlluminance(notification.Payload);

        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
    }
}
