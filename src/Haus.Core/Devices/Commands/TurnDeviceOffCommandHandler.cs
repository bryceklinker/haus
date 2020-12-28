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
    public class TurnDeviceOffCommand : ICommand
    {
        public long DeviceId { get; }

        public TurnDeviceOffCommand(long deviceId)
        {
            DeviceId = deviceId;
        }
    }

    internal class TurnDeviceOffCommandHandler : AsyncRequestHandler<TurnDeviceOffCommand>, ICommandHandler<TurnDeviceOffCommand>
    {
        private readonly HausDbContext _context;
        private readonly IDomainEventBus _domainEventBus;

        public TurnDeviceOffCommandHandler(HausDbContext context, IDomainEventBus domainEventBus)
        {
            _context = context;
            _domainEventBus = domainEventBus;
        }

        protected override async Task Handle(TurnDeviceOffCommand request, CancellationToken cancellationToken)
        {
            var device = await _context.FindByIdOrThrowAsync<DeviceEntity>(request.DeviceId, cancellationToken)
                .ConfigureAwait(false);
            device.TurnOff(_domainEventBus);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _domainEventBus.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}