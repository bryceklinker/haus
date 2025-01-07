using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands;

public record UpdateDeviceCommand(DeviceModel Model) : UpdateEntityCommand<DeviceModel>(Model);

internal class UpdateDeviceCommandHandler(
    IValidator<DeviceModel> validator,
    IHausBus hausBus,
    IDeviceCommandRepository repository)
    : ICommandHandler<UpdateDeviceCommand>
{
    public async Task Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        await validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
            .ConfigureAwait(false);

        var device = await repository.GetById(request.Id, cancellationToken)
            .ConfigureAwait(false);

        device.UpdateFromModel(request.Model, hausBus);
        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);

        await hausBus.PublishAsync(RoutableEvent.FromEvent(new DeviceUpdatedEvent(device.ToModel())),
                cancellationToken)
            .ConfigureAwait(false);
    }
}