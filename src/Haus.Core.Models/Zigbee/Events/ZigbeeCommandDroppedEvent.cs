using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeCommandDroppedEvent(string ExternalId, string Reason) : IHausEventCreator<ZigbeeCommandDroppedEvent>
{
    public const string Type = "zigbee_command_dropped";

    public HausEvent<ZigbeeCommandDroppedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
