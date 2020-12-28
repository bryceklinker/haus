using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Cqrs.Events;

namespace Haus.Core.Common.Events
{
    public class RoutableCommand : IEvent
    {
        public HausCommand HausCommand { get; }

        public RoutableCommand(HausCommand hausCommand)
        {
            HausCommand = hausCommand;
        }

        public static RoutableCommand FromEvent<T>(T command)
            where T : IHausCommandCreator<T>
        {
            return new RoutableCommand(command.AsHausCommand());
        }
    }
}