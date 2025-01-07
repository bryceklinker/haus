using System;

namespace Haus.Utilities.Common.Cli;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(string groupName, string commandName) : Attribute
{
    public string CommandName { get; } = commandName;

    public string GroupName { get; } = groupName;
}