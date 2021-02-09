using System;
using System.Threading.Tasks;
using Haus.Core.Models;

namespace Haus.Udp.Client
{
    public interface IHausUdpSubscription
    {
        Guid Id { get; }
        Task ExecuteAsync(byte[] bytes);
        Task UnsubscribeAsync();
    }

    internal class HausUdpSubscription<T> : IHausUdpSubscription
    {
        private readonly Func<T, Task> _handler;
        private readonly Func<IHausUdpSubscription, Task> _unsubscribe;
        public Guid Id { get; } = Guid.NewGuid();

        public HausUdpSubscription(Func<T, Task> handler, Func<IHausUdpSubscription, Task> unsubscribe)
        {
            _handler = handler;
            _unsubscribe = unsubscribe;
        }
        
        public Task ExecuteAsync(byte[] bytes)
        {
            return HausJsonSerializer.TryDeserialize(bytes, out T value) 
                ? _handler.Invoke(value) 
                : Task.CompletedTask;
        }

        public Task UnsubscribeAsync()
        {
            return _unsubscribe.Invoke(this);
        }
    }
}