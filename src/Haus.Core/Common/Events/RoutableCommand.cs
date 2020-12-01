using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Common.Events
{
    public class RoutableCommand : IEvent
    {
        public HausCommand HausCommand { get; }

        public RoutableCommand(HausCommand hausCommand)
        {
            HausCommand = hausCommand;
        }
    }
}