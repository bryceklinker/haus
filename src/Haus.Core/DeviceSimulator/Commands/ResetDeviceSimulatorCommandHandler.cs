using System.Threading;
using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.State;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.DeviceSimulator.Commands;

public record ResetDeviceSimulatorCommand : ICommand;

public class ResetDeviceSimulatorCommandHandler(IDeviceSimulatorStore store)
    : ICommandHandler<ResetDeviceSimulatorCommand>
{
    public Task Handle(ResetDeviceSimulatorCommand request, CancellationToken cancellationToken)
    {
        store.Publish(store.Current.Reset());
        return Task.CompletedTask;
    }
}