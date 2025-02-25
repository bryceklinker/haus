using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands;

public record ChangeDeviceLightingCommand(long DeviceId, LightingModel Lighting) : ICommand;

internal class ChangeDeviceLightingCommandHandler(IDomainEventBus domainEventBus, IDeviceCommandRepository repository)
    : ICommandHandler<ChangeDeviceLightingCommand>
{
    public async Task Handle(ChangeDeviceLightingCommand request, CancellationToken cancellationToken)
    {
        var device = await repository.GetById(request.DeviceId, cancellationToken).ConfigureAwait(false);

        if (!device.IsLight)
            throw new InvalidOperationException($"Device with id {device.Id} is not a light");

        var lighting = LightingEntity.FromModel(request.Lighting);
        device.ChangeLighting(lighting, domainEventBus);
        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);

        await domainEventBus.FlushAsync(cancellationToken);
    }
}
