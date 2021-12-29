using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands;

public record TurnDeviceOnCommand(long DeviceId) : ICommand;

internal class TurnDeviceOnCommandHandler : AsyncRequestHandler<TurnDeviceOnCommand>,
    ICommandHandler<TurnDeviceOnCommand>
{
    private readonly IDeviceCommandRepository _repository;
    private readonly IDomainEventBus _domainEventBus;

    public TurnDeviceOnCommandHandler(IDomainEventBus domainEventBus, IDeviceCommandRepository repository)
    {
        _domainEventBus = domainEventBus;
        _repository = repository;
    }

    protected override async Task Handle(TurnDeviceOnCommand request, CancellationToken cancellationToken)
    {
        var device = await _repository.GetById(request.DeviceId, cancellationToken).ConfigureAwait(false);
        device.TurnOn(_domainEventBus);
        await _repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
        await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}