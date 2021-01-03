using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting;
using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record ChangeDeviceLightingCommand(long DeviceId, LightingModel Lighting) : ICommand;

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
            var device = await _context.FindByIdOrThrowAsync<DeviceEntity>(request.DeviceId, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (!device.IsLight)
                throw new InvalidOperationException($"Device with id {device.Id} is not a light");

            var lighting = LightingEntity.FromModel(request.Lighting);
            device.ChangeLighting(lighting, _domainEventBus);
            await _context.SaveChangesAsync(cancellationToken);

            await _domainEventBus.FlushAsync(cancellationToken);
        }
    }
}