using System.Threading;
using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.Events;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.DeviceSimulator;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.DeviceSimulator.Commands;

public record CreateSimulatedDeviceCommand(SimulatedDeviceModel Model) : ICommand;

internal class CreateSimulatedDeviceCommandHandler(IDeviceSimulatorStore store, IHausBus hausBus)
    : ICommandHandler<CreateSimulatedDeviceCommand>
{
    public async Task Handle(CreateSimulatedDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = SimulatedDeviceEntity.Create(request.Model);
        store.Publish(store.Current.AddSimulatedDevice(device));
        await hausBus.PublishAsync(SimulatedEvent.FromEvent(device.ToDeviceDiscoveredModel()), cancellationToken)
            .ConfigureAwait(false);
    }
}