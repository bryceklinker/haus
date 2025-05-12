using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Cqrs.Events;

namespace Haus.Core.DeviceSimulator.Events;

public record SimulatedEvent(HausEvent HausEvent) : IEvent
{
    public static SimulatedEvent FromEvent<T>(T @event)
        where T : IHausEventCreator<T>
    {
        return new SimulatedEvent(@event.AsHausEvent());
    }
}
