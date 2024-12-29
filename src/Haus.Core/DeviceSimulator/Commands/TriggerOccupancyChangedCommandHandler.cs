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

public class TriggerOccupancyChangedCommandHandler : ICommandHandler<TriggerOccupancyChangedCommand>
{
    private readonly IDeviceSimulatorStore _store;
    private readonly IHausBus _hausBus;

    public TriggerOccupancyChangedCommandHandler(IDeviceSimulatorStore store, IHausBus hausBus)
    {
        _store = store;
        _hausBus = hausBus;
    }

    public async Task Handle(TriggerOccupancyChangedCommand request, CancellationToken cancellationToken)
    {
        _store.PublishNext(s => s.ChangeOccupancy(request.SimulatorId));

        var device = _store.GetDeviceById(request.SimulatorId);
        if (device == null)
            throw new SimulatorNotFoundException(request.SimulatorId);

        var simulatedEvent = SimulatedEvent.FromEvent(new OccupancyChangedModel(device.Id, device.IsOccupied));
        await _hausBus.PublishAsync(simulatedEvent, cancellationToken).ConfigureAwait(false);
    }
}