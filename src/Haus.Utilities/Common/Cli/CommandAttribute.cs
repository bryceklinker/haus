using System;

namespace Haus.Utilities.Common.Cli
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string CommandName { get; }
        
        public string GroupName { get; }

        public CommandAttribute(string groupName, string commandName)
        {
            CommandName = commandName;
            GroupName = groupName;
        }
    }
}