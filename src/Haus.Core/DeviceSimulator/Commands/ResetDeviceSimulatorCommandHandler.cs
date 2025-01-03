using System.Threading;
using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.State;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.DeviceSimulator.Commands;

public record ResetDeviceSimulatorCommand : ICommand;

public class ResetDeviceSimulatorCommandHandler : AsyncRequestHandler<ResetDeviceSimulatorCommand>,
    ICommandHandler<ResetDeviceSimulatorCommand>
{
    private readonly IDeviceSimulatorStore _store;

    public ResetDeviceSimulatorCommandHandler(IDeviceSimulatorStore store)
    {
        _store = store;
    }

    protected override Task Handle(ResetDeviceSimulatorCommand request, CancellationToken cancellationToken)
    {
        _store.Publish(_store.Current.Reset());
        return Task.CompletedTask;
    }
}