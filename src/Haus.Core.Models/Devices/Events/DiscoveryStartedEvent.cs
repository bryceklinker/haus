using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events
{
    public class DiscoveryStartedEvent : IHausEventCreator<DiscoveryStartedEvent>
    {
        public const string Type = "discovery_started";
        public HausEvent<DiscoveryStartedEvent> AsHausEvent() => new(Type, this);
    }
}