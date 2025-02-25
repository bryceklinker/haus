using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands;

public record ChangeDeviceLightingConstraintsCommand(long DeviceId, LightingConstraintsModel Model) : ICommand;

public class ChangeDeviceLightingConstraintsCommandHandler(IDeviceCommandRepository repository, IHausBus hausBus)
    : ICommandHandler<ChangeDeviceLightingConstraintsCommand>
{
    public async Task Handle(ChangeDeviceLightingConstraintsCommand request, CancellationToken cancellationToken)
    {
        var device = await repository.GetById(request.DeviceId, cancellationToken).ConfigureAwait(false);
        device.UpdateFromLightingConstraints(request.Model, hausBus);
        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
        await hausBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
