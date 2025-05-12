using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Utilities.Common.Cli;

public record ExecuteCommand(string[] Args) : ICommand;

public class ExecuteCommandHandler(IHausBus hausBus, ICommandFactory commandFactory) : ICommandHandler<ExecuteCommand>
{
    public async Task Handle(ExecuteCommand request, CancellationToken cancellationToken)
    {
        var command = commandFactory.Create(request.Args);
        await hausBus.ExecuteCommandAsync(command, cancellationToken);
    }
}
