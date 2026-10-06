using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;

public interface IClusterDestinationResolver
{
    ApsDestination ResolveDestination(ushort clusterId);
}
