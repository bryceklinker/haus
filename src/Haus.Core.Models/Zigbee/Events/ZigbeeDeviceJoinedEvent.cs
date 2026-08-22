using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeDeviceJoinedEvent(string IeeeAddress, ushort NetworkAddress)
    : IHausEventCreator<ZigbeeDeviceJoinedEvent>
{
    public const string Type = "zigbee_device_joined";

    public HausEvent<ZigbeeDeviceJoinedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
