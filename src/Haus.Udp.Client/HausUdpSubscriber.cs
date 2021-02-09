using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Udp.Client
{
    public interface IHausUdpSubscriber : IDisposable
    {
        IHausUdpSubscription Subscribe<T>(Action<T> handler);
        IHausUdpSubscription Subscribe<T>(Func<T, Task> handler);
    }

    internal class HausUdpSubscriber : IHausUdpSubscriber
    {
        private readonly UdpClient _client;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ConcurrentDictionary<Guid, IHausUdpSubscription> _subscriptions;
        private Task _listeningTask;
        private bool _isStarted;

        public HausUdpSubscriber(UdpClient client)
        {
            _client = client;
            _cancellationTokenSource = new CancellationTokenSource();
            _subscriptions = new ConcurrentDictionary<Guid, IHausUdpSubscription>();
            _isStarted = false;
        }

        public IHausUdpSubscription Subscribe<T>(Action<T> handler)
        {
            return Subscribe<T>(value =>
            {
                handler.Invoke(value);
                return Task.CompletedTask;
            });
        }

        public IHausUdpSubscription Subscribe<T>(Func<T, Task> handler)
        {
            EnsureListenerStarted();

            var subscription = new HausUdpSubscription<T>(handler, Unsubscribe);
            _subscriptions.GetOrAdd(subscription.Id, subscription);
            return subscription;
        }

        private Task Unsubscribe(IHausUdpSubscription subscription)
        {
            _subscriptions.TryRemove(subscription.Id, out subscription);
            return Task.CompletedTask;
        }
        
        public void Dispose()
        {
            _client.Dispose();
            _cancellationTokenSource.Cancel();
            _listeningTask.Dispose();
        }

        private void EnsureListenerStarted()
        {
            if (_isStarted)
                return;

            _isStarted = true;
            _listeningTask = StartListening(_cancellationTokenSource.Token);
        }
        
        private async Task StartListening(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var data = await _client.ReceiveAsync().ConfigureAwait(false);
                var tasks = _subscriptions.Values.Select(s => s.ExecuteAsync(data.Buffer));
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }
    }
}