using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeCommandSentEvent(ushort? NetworkAddress, string? IeeeAddress, ushort ClusterId, byte CommandId)
    : IHausEventCreator<ZigbeeCommandSentEvent>
{
    public const string Type = "zigbee_command_sent";

    public HausEvent<ZigbeeCommandSentEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
