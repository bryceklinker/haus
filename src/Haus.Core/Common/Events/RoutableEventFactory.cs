using System;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;

namespace Haus.Core.Common.Events;

public interface IRoutableEventFactory
{
    RoutableEvent Create(ArraySegment<byte> bytes);
}

public class RoutableEventFactory : IRoutableEventFactory
{
    public RoutableEvent Create(ArraySegment<byte> bytes)
    {
        if (!HausJsonSerializer.TryDeserialize(bytes, out HausEvent hausEvent))
            return null;

        return hausEvent.Type switch
        {
            DeviceDiscoveredEvent.Type => CreateRoutableEvent<DeviceDiscoveredEvent>(bytes),
            MultiSensorChanged.Type => CreateRoutableEvent<MultiSensorChanged>(bytes),
            OccupancyChangedModel.Type => CreateRoutableEvent<OccupancyChangedModel>(bytes),
            _ => null
        };
    }

    private static RoutableEvent CreateRoutableEvent<T>(ArraySegment<byte> bytes)
    {
        var hausEvent = HausJsonSerializer.Deserialize<HausEvent<T>>(bytes);
        return new RoutableEvent<T>(hausEvent);
    }
}