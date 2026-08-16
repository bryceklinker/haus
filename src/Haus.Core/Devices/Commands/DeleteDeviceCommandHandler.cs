using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs;
using Haus.Cqrs.Commands;

namespace Haus.Core.Devices.Commands;

public record DeleteDeviceCommand(long Id) : ICommand;

internal class DeleteDeviceCommandHandler(
    IDeviceCommandRepository deviceRepository,
    IRoomCommandRepository roomRepository,
    IHausBus hausBus
) : ICommandHandler<DeleteDeviceCommand>
{
    public async Task Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetById(request.Id, cancellationToken).ConfigureAwait(false);

        if (device.Room != null)
        {
            var room = await roomRepository.GetByIdAsync(device.Room.Id, cancellationToken).ConfigureAwait(false);
            room.RemoveDevice(device);
        }

        var model = device.ToModel();
        await deviceRepository.DeleteAsync(device, cancellationToken).ConfigureAwait(false);

        await hausBus
            .PublishAsync(RoutableEvent.FromEvent(new DeviceDeletedEvent(model)), cancellationToken)
            .ConfigureAwait(false);
    }
}
