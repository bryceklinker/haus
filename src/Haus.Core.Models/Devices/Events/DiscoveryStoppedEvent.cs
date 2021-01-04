using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Devices.Events
{
    public class DiscoveryStoppedEvent : IHausEventCreator<DiscoveryStoppedEvent>
    {
        public const string Type = "discovery_stopped";
        public HausEvent<DiscoveryStoppedEvent> AsHausEvent() => new(Type, this);
    }
}