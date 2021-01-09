using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record ChangeDeviceLightingCommand(long DeviceId, LightingModel Lighting) : ICommand;

    internal class ChangeDeviceLightingCommandHandler : AsyncRequestHandler<ChangeDeviceLightingCommand>, ICommandHandler<ChangeDeviceLightingCommand>
    {
        private readonly IDeviceCommandRepository _repository;
        private readonly IDomainEventBus _domainEventBus;

        public ChangeDeviceLightingCommandHandler(IDomainEventBus domainEventBus, IDeviceCommandRepository repository)
        {
            _domainEventBus = domainEventBus;
            _repository = repository;
        }

        protected override async Task Handle(ChangeDeviceLightingCommand request, CancellationToken cancellationToken)
        {
            var device = await _repository.GetById(request.DeviceId, cancellationToken)
                .ConfigureAwait(false);

            if (!device.IsLight)
                throw new InvalidOperationException($"Device with id {device.Id} is not a light");

            var lighting = LightingEntity.FromModel(request.Lighting);
            device.ChangeLighting(lighting, _domainEventBus);
            await _repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);

            await _domainEventBus.FlushAsync(cancellationToken);
        }
    }
}