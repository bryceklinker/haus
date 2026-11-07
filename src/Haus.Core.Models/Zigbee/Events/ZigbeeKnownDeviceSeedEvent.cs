using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Models.Zigbee.Events;

public record ZigbeeKnownDeviceSeedEntry(
    string ExternalId,
    ushort NetworkAddress,
    DeviceEndpointModel[] Endpoints,
    bool IsSleepy
);

public record ZigbeeKnownDeviceSeedEvent(ZigbeeKnownDeviceSeedEntry[] Devices)
    : IHausEventCreator<ZigbeeKnownDeviceSeedEvent>
{
    public const string Type = "zigbee_known_device_seed";

    public HausEvent<ZigbeeKnownDeviceSeedEvent> AsHausEvent()
    {
        return new(Type, this);
    }
}
