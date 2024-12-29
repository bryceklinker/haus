using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Utilities.Common.Cli;

public record ExecuteCommand(string[] Args) : ICommand;

public class ExecuteCommandHandler : ICommandHandler<ExecuteCommand>
{
    private readonly IHausBus _hausBus;
    private readonly ICommandFactory _commandFactory;

    public ExecuteCommandHandler(IHausBus hausBus, ICommandFactory commandFactory)
    {
        _hausBus = hausBus;
        _commandFactory = commandFactory;
    }

    public async Task Handle(ExecuteCommand request, CancellationToken cancellationToken)
    {
        var command = _commandFactory.Create(request.Args);
        await _hausBus.ExecuteCommandAsync(command, cancellationToken);
    }
}