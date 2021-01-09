using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record ChangeDeviceLightingConstraintsCommand(long DeviceId, LightingConstraintsModel Model) : ICommand;

    internal class ChangeDeviceLightingConstraintsCommandHandler : AsyncRequestHandler<ChangeDeviceLightingConstraintsCommand>, ICommandHandler<ChangeDeviceLightingConstraintsCommand>
    {
        private readonly IDeviceCommandRepository _repository;
        private readonly IHausBus _hausBus;

        public ChangeDeviceLightingConstraintsCommandHandler(IDeviceCommandRepository repository, IHausBus hausBus)
        {
            _repository = repository;
            _hausBus = hausBus;
        }

        protected override async Task Handle(ChangeDeviceLightingConstraintsCommand request, CancellationToken cancellationToken)
        {
            var device = await _repository.GetById(request.DeviceId, cancellationToken).ConfigureAwait(false);
            var constraints = LightingConstraintsEntity.FromModel(request.Model);
            device.ChangeLightingConstraints(constraints, _hausBus);
            await _repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
            await _hausBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}