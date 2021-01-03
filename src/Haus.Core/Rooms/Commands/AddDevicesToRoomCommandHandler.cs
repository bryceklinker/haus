using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using MediatR;

namespace Haus.Core.Rooms.Commands
{
    public record AddDevicesToRoomCommand(long RoomId, params long[] DeviceIds) : ICommand;

    internal class AddDevicesToRoomCommandHandler : AsyncRequestHandler<AddDevicesToRoomCommand>, ICommandHandler<AddDevicesToRoomCommand>
    {
        private readonly HausDbContext _context;
        private readonly ICommandRoomRepository _repository;
        private readonly IDomainEventBus _domainEventBus;

        public AddDevicesToRoomCommandHandler(HausDbContext context, IDomainEventBus domainEventBus, ICommandRoomRepository repository)
        {
            _context = context;
            _domainEventBus = domainEventBus;
            _repository = repository;
        }

        protected override async Task Handle(AddDevicesToRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);

            var devices = await EnsureAllDevicesExist(request.DeviceIds, cancellationToken)
                .ConfigureAwait(false);
            room.AddDevices(devices, _domainEventBus);
            await _repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        }

        private async Task<DeviceEntity[]> EnsureAllDevicesExist(long[] deviceIds, CancellationToken token)
        {
            var devices = await _context.FindAllById<DeviceEntity>(deviceIds, token);
            if (devices.Length == deviceIds.Length)
                return devices;

            var existingDeviceIds = devices.Select(d => d.Id).ToArray();
            var missingDeviceIds = deviceIds.Where(id => existingDeviceIds.Missing(id));
            throw new EntityNotFoundException<DeviceEntity>(missingDeviceIds);
        }
    }
}