using System;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee.Events;

namespace Haus.Core.Common.Events;

public interface IRoutableEventFactory
{
    RoutableEvent? Create(ArraySegment<byte> bytes);
}

public class RoutableEventFactory : IRoutableEventFactory
{
    public RoutableEvent? Create(ArraySegment<byte> bytes)
    {
        if (!HausJsonSerializer.TryDeserialize(bytes, out HausEvent? hausEvent))
            return null;

        return hausEvent?.Type switch
        {
            DeviceDiscoveredEvent.Type => CreateRoutableEvent<DeviceDiscoveredEvent>(bytes),
            MultiSensorChanged.Type => CreateRoutableEvent<MultiSensorChanged>(bytes),
            OccupancyChangedModel.Type => CreateRoutableEvent<OccupancyChangedModel>(bytes),
            ZigbeeConnectionStatusChangedEvent.Type => CreateRoutableEvent<ZigbeeConnectionStatusChangedEvent>(bytes),
            ZigbeeDeviceJoinedEvent.Type => CreateRoutableEvent<ZigbeeDeviceJoinedEvent>(bytes),
            ZigbeeDeviceInfoDiscoveredEvent.Type => CreateRoutableEvent<ZigbeeDeviceInfoDiscoveredEvent>(bytes),
            ZigbeeAttributeReportReceivedEvent.Type => CreateRoutableEvent<ZigbeeAttributeReportReceivedEvent>(bytes),
            ZigbeeCommandSentEvent.Type => CreateRoutableEvent<ZigbeeCommandSentEvent>(bytes),
            ZigbeeTransportErrorEvent.Type => CreateRoutableEvent<ZigbeeTransportErrorEvent>(bytes),
            ZigbeeCommandDroppedEvent.Type => CreateRoutableEvent<ZigbeeCommandDroppedEvent>(bytes),
            _ => null,
        };
    }

    private static RoutableEvent? CreateRoutableEvent<T>(ArraySegment<byte> bytes)
    {
        var hausEvent = HausJsonSerializer.Deserialize<HausEvent<T>>(bytes);
        return hausEvent != null ? new RoutableEvent<T>(hausEvent) : null;
    }
}
