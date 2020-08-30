using MassTransit;

namespace Haus.ServiceBus.Subscribe
{
    public interface IHausServiceBusSubscriber<in TPayload> : IConsumer<TPayload> 
        where TPayload : class
    {
        
    }
}