using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.DomainEvents;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public class ChangeDeviceLightingCommand : ICommand
    {
        public long DeviceId { get; }
        public LightingModel Lighting { get; }

        public ChangeDeviceLightingCommand(long deviceId, LightingModel lighting)
        {
            DeviceId = deviceId;
            Lighting = lighting;
        }
    }

    internal class ChangeDeviceLightingCommandHandler : AsyncRequestHandler<ChangeDeviceLightingCommand>, ICommandHandler<ChangeDeviceLightingCommand>
    {
        private readonly HausDbContext _context;
        private readonly IDomainEventBus _domainEventBus;

        public ChangeDeviceLightingCommandHandler(HausDbContext context, IDomainEventBus domainEventBus)
        {
            _context = context;
            _domainEventBus = domainEventBus;
        }

        protected override async Task Handle(ChangeDeviceLightingCommand request, CancellationToken cancellationToken)
        {
            var device = await _context.FindByIdOrThrowAsync<DeviceEntity>(request.DeviceId, cancellationToken)
                .ConfigureAwait(false);

            if (!device.IsLight)
                throw new InvalidOperationException($"Device with id {device.Id} is not a light");

            var lighting = Lighting.FromModel(request.Lighting);
            device.ChangeLighting(lighting, _domainEventBus);
            await _context.SaveChangesAsync(cancellationToken);

            await _domainEventBus.FlushAsync(cancellationToken);
        }
    }
}