using System.Collections.Generic;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeDeviceInfoDiscoveredEvent(
    string IeeeAddress,
    string ManufacturerName,
    string ModelIdentifier,
    IReadOnlyList<ZigbeeEndpointModel> Endpoints
) : IHausEventCreator<ZigbeeDeviceInfoDiscoveredEvent>
{
    public const string Type = "zigbee_device_info_discovered";

    public HausEvent<ZigbeeDeviceInfoDiscoveredEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
