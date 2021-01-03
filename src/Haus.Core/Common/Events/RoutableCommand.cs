using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Cqrs.Events;

namespace Haus.Core.Common.Events
{
    public record RoutableCommand(HausCommand HausCommand) : IEvent
    {
        public static RoutableCommand FromEvent<T>(T command)
            where T : IHausCommandCreator<T>
        {
            return new(command.AsHausCommand());
        }
    }
}