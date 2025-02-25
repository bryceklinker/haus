using System.Threading;
using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Events;
using Haus.Core.DeviceSimulator.Exceptions;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.DeviceSimulator.Commands;

public record TriggerOccupancyChangedCommand(string SimulatorId) : ICommand;

public class TriggerOccupancyChangedCommandHandler(IDeviceSimulatorStore store, IHausBus hausBus)
    : ICommandHandler<TriggerOccupancyChangedCommand>
{
    public async Task Handle(TriggerOccupancyChangedCommand request, CancellationToken cancellationToken)
    {
        store.PublishNext(s => s.ChangeOccupancy(request.SimulatorId));

        var device = store.GetDeviceById(request.SimulatorId);
        if (device == null)
            throw new SimulatorNotFoundException(request.SimulatorId);

        var simulatedEvent = SimulatedEvent.FromEvent(new OccupancyChangedModel(device.Id, device.IsOccupied));
        await hausBus.PublishAsync(simulatedEvent, cancellationToken).ConfigureAwait(false);
    }
}
