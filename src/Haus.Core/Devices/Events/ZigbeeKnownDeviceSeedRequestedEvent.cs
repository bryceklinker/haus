using Haus.Core.Models.Zigbee.Events;
using Haus.Cqrs.Events;

namespace Haus.Core.Devices.Events;

public record ZigbeeKnownDeviceSeedRequestedEvent(ZigbeeKnownDeviceSeedEvent Seed) : IEvent;
