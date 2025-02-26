using System;
using System.Reflection;

namespace Haus.Utilities.Common.Cli;

public class KnownCommand(Type type)
{
    public Type CommandType { get; } = type;
    private readonly CommandAttribute? _attribute = type.GetCustomAttribute<CommandAttribute>();

    public bool Matches(string group, string command)
    {
        ArgumentNullException.ThrowIfNull(_attribute);

        return group == _attribute.GroupName && command == _attribute.CommandName;
    }
}
