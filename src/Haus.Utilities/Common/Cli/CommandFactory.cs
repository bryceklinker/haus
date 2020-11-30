using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Haus.Utilities.Common.Cli
{
    public interface ICommandFactory
    {
        object Create(string[] args);
    }

    public class CommandFactory : ICommandFactory
    {
        private readonly ILogger<CommandFactory> _logger;
        private static readonly Lazy<KnownCommand[]> KnownCommands = new Lazy<KnownCommand[]>(DiscoverKnownCommands);

        private IEnumerable<KnownCommand> Commands => KnownCommands.Value;

        public CommandFactory(ILogger<CommandFactory> logger)
        {
            _logger = logger;
        }

        public object Create(string[] args)
        {
            _logger.LogInformation("Args: {args}", string.Join(" ", args));
            var groupName = args[0];
            var commandName = args[1];
            var command = Commands.SingleOrDefault(c => c.Matches(groupName, commandName));
            if (command != null)
                return Activator.CreateInstance(command.CommandType);

            throw new CommandNotFoundException();
        }

        private static KnownCommand[] DiscoverKnownCommands()
        {
            return typeof(CommandFactory).Assembly.GetExportedTypes()
                .Where(t => t.GetCustomAttribute<CommandAttribute>() != null)
                .Select(t => new KnownCommand(t))
                .ToArray();
        }
    }
}