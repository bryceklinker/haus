using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Haus.Cqrs.Commands;
using Microsoft.Extensions.Logging;

namespace Haus.Utilities.Common.Cli;

public interface ICommandFactory
{
    ICommand Create(string[] args);
}

public class CommandFactory(ILogger<CommandFactory> logger) : ICommandFactory
{
    private static readonly Lazy<KnownCommand[]> KnownCommands = new(DiscoverKnownCommands);

    private IEnumerable<KnownCommand> Commands => KnownCommands.Value;

    public ICommand Create(string[] args)
    {
        logger.LogInformation("Args: {Args}", string.Join(" ", args));
        var groupName = args[0];
        var commandName = args[1];
        var command = Commands.SingleOrDefault(c => c.Matches(groupName, commandName));
        if (command != null)
            return Activator.CreateInstance(command.CommandType) as ICommand;

        throw new CommandNotFoundException();
    }

    private static KnownCommand[] DiscoverKnownCommands()
    {
        return typeof(CommandFactory)
            .Assembly.GetExportedTypes()
            .Where(t => t.GetCustomAttribute<CommandAttribute>() != null)
            .Select(t => new KnownCommand(t))
            .ToArray();
    }
}
