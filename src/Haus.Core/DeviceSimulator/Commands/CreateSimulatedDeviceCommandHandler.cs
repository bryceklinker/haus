using System.Threading;
using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.Events;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.DeviceSimulator;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.DeviceSimulator.Commands
{
    public class CreateSimulatedDeviceCommand : ICommand
    {
        public CreateSimulatedDeviceModel Model { get; }

        public CreateSimulatedDeviceCommand(CreateSimulatedDeviceModel model)
        {
            Model = model;
        }
    }

    internal class CreateSimulatedDeviceCommandHandler : AsyncRequestHandler<CreateSimulatedDeviceCommand>, ICommandHandler<CreateSimulatedDeviceCommand>
    {
        private readonly IDeviceSimulatorStore _store;
        private readonly IHausBus _hausBus;

        public CreateSimulatedDeviceCommandHandler(IDeviceSimulatorStore store, IHausBus hausBus)
        {
            _store = store;
            _hausBus = hausBus;
        }

        protected override async Task Handle(CreateSimulatedDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = SimulatedDeviceEntity.Create(request.Model);
            _store.Publish(_store.Current.AddSimulatedDevice(device));
            await _hausBus.PublishAsync(SimulatedEvent.FromEvent(device.ToDeviceDiscoveredModel()), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}