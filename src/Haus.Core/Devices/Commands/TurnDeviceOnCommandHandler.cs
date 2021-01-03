using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record TurnDeviceOnCommand(long DeviceId) : ICommand;

    internal class TurnDeviceOnCommandHandler : AsyncRequestHandler<TurnDeviceOnCommand>, ICommandHandler<TurnDeviceOnCommand>
    {
        private readonly HausDbContext _context;
        private readonly IDomainEventBus _domainEventBus;

        public TurnDeviceOnCommandHandler(HausDbContext context, IDomainEventBus domainEventBus)
        {
            _context = context;
            _domainEventBus = domainEventBus;
        }

        protected override async Task Handle(TurnDeviceOnCommand request, CancellationToken cancellationToken)
        {
            var device = await _context.FindByIdOrThrowAsync<DeviceEntity>(request.DeviceId, cancellationToken: cancellationToken).ConfigureAwait(false);
            device.TurnOn(_domainEventBus);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}