using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record TurnDeviceOffCommand(long DeviceId) : ICommand;

    internal class TurnDeviceOffCommandHandler : AsyncRequestHandler<TurnDeviceOffCommand>, ICommandHandler<TurnDeviceOffCommand>
    {
        private readonly IDeviceCommandRepository _repository;
        private readonly IDomainEventBus _domainEventBus;

        public TurnDeviceOffCommandHandler(IDomainEventBus domainEventBus, IDeviceCommandRepository repository)
        {
            _domainEventBus = domainEventBus;
            _repository = repository;
        }

        protected override async Task Handle(TurnDeviceOffCommand request, CancellationToken cancellationToken)
        {
            var device = await _repository.GetById(request.DeviceId, cancellationToken)
                .ConfigureAwait(false);
            device.TurnOff(_domainEventBus);
            await _repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);
            await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}