using System;
using System.Reflection;

namespace Haus.Utilities.Common.Cli;

public class KnownCommand
{
    public Type CommandType { get; }
    private readonly CommandAttribute _attribute;

    public KnownCommand(Type type)
    {
        CommandType = type;
        _attribute = type.GetCustomAttribute<CommandAttribute>();
    }

    public bool Matches(string group, string command)
    {
        return group == _attribute.GroupName
               && command == _attribute.CommandName;
    }
}