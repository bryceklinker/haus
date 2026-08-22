using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeConnectionStatusChangedEvent(
    bool IsConnected,
    ZigbeeNetworkConfigModel? NetworkConfig,
    string? Reason
) : IHausEventCreator<ZigbeeConnectionStatusChangedEvent>
{
    public const string Type = "zigbee_connection_status_changed";

    public HausEvent<ZigbeeConnectionStatusChangedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
