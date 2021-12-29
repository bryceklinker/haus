using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands;

public record ChangeDeviceLightingConstraintsCommand(long DeviceId, LightingConstraintsModel Model) : ICommand;

public class ChangeDeviceLightingConstraintsCommandHandler :
    AsyncRequestHandler<ChangeDeviceLightingConstraintsCommand>, ICommandHandler<ChangeDeviceLightingConstraintsCommand>
{
    private readonly IDeviceCommandRepository _repository;
    private readonly IHausBus _hausBus;

    public ChangeDeviceLightingConstraintsCommandHandler(IDeviceCommandRepository repository, IHausBus hausBus)
    {
        _repository = repository;
        _hausBus = hausBus;
    }

    protected override async Task Handle(ChangeDeviceLightingConstraintsCommand request,
        CancellationToken cancellationToken)
    {
        var device = await _repository.GetById(request.DeviceId, cancellationToken).ConfigureAwait(false);
        device.UpdateFromLightingConstraints(request.Model, _hausBus);
        await _repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
        await _hausBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}