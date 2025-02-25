using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands;

public record TurnDeviceOffCommand(long DeviceId) : ICommand;

internal class TurnDeviceOffCommandHandler(IDomainEventBus domainEventBus, IDeviceCommandRepository repository)
    : ICommandHandler<TurnDeviceOffCommand>
{
    public async Task Handle(TurnDeviceOffCommand request, CancellationToken cancellationToken)
    {
        var device = await repository.GetById(request.DeviceId, cancellationToken).ConfigureAwait(false);
        device.TurnOff(domainEventBus);
        await repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
        await domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
