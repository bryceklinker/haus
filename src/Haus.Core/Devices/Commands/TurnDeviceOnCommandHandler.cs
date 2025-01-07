using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands;

public record TurnDeviceOnCommand(long DeviceId) : ICommand;

internal class TurnDeviceOnCommandHandler(IDomainEventBus domainEventBus, IDeviceCommandRepository repository)
    : ICommandHandler<TurnDeviceOnCommand>
{
    public async Task Handle(TurnDeviceOnCommand request, CancellationToken cancellationToken)
    {
        var device = await repository.GetById(request.DeviceId, cancellationToken).ConfigureAwait(false);
        device.TurnOn(domainEventBus);
        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}