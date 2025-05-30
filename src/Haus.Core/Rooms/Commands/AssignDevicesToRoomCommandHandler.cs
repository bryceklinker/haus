using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Rooms.Commands;

public record AssignDevicesToRoomCommand(long RoomId, params long[] DeviceIds) : ICommand;

internal class AssignDevicesToRoomCommandHandler(
    HausDbContext context,
    IRoomCommandRepository repository,
    IHausBus hausBus
) : ICommandHandler<AssignDevicesToRoomCommand>
{
    public async Task Handle(AssignDevicesToRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await repository.GetByIdAsync(request.RoomId, cancellationToken).ConfigureAwait(false);

        var devices = await EnsureAllDevicesExist(request.DeviceIds, cancellationToken).ConfigureAwait(false);
        room.AddDevices(devices, hausBus);
        await repository.SaveAsync(room, cancellationToken).ConfigureAwait(false);
        await hausBus
            .PublishAsync(
                RoutableEvent.FromEvent(new DevicesAssignedToRoomEvent(request.RoomId, request.DeviceIds)),
                cancellationToken
            )
            .ConfigureAwait(false);
        await hausBus.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<DeviceEntity[]> EnsureAllDevicesExist(long[] deviceIds, CancellationToken token)
    {
        var devices = await context.FindAllById<DeviceEntity>(deviceIds, token).ConfigureAwait(false);
        if (devices.Length == deviceIds.Length)
            return devices;

        var existingDeviceIds = devices.Select(d => d.Id).ToArray();
        var missingDeviceIds = deviceIds.Where(id => existingDeviceIds.Missing(id));
        throw new EntityNotFoundException<DeviceEntity>(missingDeviceIds);
    }
}
