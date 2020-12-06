using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Commands
{
    public class AddDevicesToRoomCommand : ICommand
    {
        public long RoomId { get; }
        public long[] DeviceIds { get; }

        public AddDevicesToRoomCommand(long roomId, params long[] deviceIds)
        {
            RoomId = roomId;
            DeviceIds = deviceIds;
        }
    }

    internal class AddDevicesToRoomCommandHandler : AsyncRequestHandler<AddDevicesToRoomCommand>, ICommandHandler<AddDevicesToRoomCommand>
    {
        private readonly HausDbContext _context;

        public AddDevicesToRoomCommandHandler(HausDbContext context)
        {
            _context = context;
        }

        protected override async Task Handle(AddDevicesToRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.FindByIdAsync<RoomEntity>(request.RoomId, cancellationToken)
                .ConfigureAwait(false);
            if (room == null)
                throw new EntityNotFoundException<RoomEntity>(request.RoomId);

            var devices = await EnsureAllDevicesExist(request.DeviceIds, cancellationToken)
                .ConfigureAwait(false);
            room.AddDevices(devices);
            await _context.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
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