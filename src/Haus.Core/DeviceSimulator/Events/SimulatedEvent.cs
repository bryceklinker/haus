using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Cqrs.Events;

namespace Haus.Core.DeviceSimulator.Events
{
    public class SimulatedEvent : IEvent
    {
        public HausEvent HausEvent { get; }
        
        public SimulatedEvent(HausEvent hausEvent)
        {
            HausEvent = hausEvent;
        }
        
        public static SimulatedEvent FromEvent<T>(T @event)
            where T : IHausEventCreator<T>
        {
            return new(@event.AsHausEvent());
        }
    }
}