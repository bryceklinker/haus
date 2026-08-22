using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeTransportErrorEvent(string ErrorType, string Message, ushort? NetworkAddress, string? IeeeAddress)
    : IHausEventCreator<ZigbeeTransportErrorEvent>
{
    public const string Type = "zigbee_transport_error";

    public HausEvent<ZigbeeTransportErrorEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
