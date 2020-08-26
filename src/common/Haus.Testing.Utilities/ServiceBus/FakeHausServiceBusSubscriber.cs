using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Haus.ServiceBus.Common;
using Haus.ServiceBus.Subscribe;

namespace Haus.Testing.Utilities.ServiceBus
{
    public class FakeHausServiceBusSubscriber : IHausServiceBusSubscriber
    {
        private readonly List<Func<ReadOnlyMemory<byte>, Task>> _subscribers;
        public bool WasDisposed { get; private set; }

        public FakeHausServiceBusSubscriber()
        {
            _subscribers = new List<Func<ReadOnlyMemory<byte>, Task>>();
            WasDisposed = false;
        }
        
        public void Dispose()
        {
            WasDisposed = true;
        }

        public void Subscribe(string queueName, Func<ReadOnlyMemory<byte>, Task> handler)
        {
            _subscribers.Add(handler);
        }

        public async Task Trigger<TPayload>(ServiceBusMessage<TPayload> message)
        {
            foreach (var subscriber in _subscribers) 
                await subscriber.Invoke(message.ToBytes());
        }
    }
}