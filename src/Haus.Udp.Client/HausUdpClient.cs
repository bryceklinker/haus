using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Microsoft.Extensions.Options;

namespace Haus.Udp.Client;

public interface IHausUdpClient : IAsyncDisposable, IDisposable
{
    Task BroadcastAsync<T>(T value);
    Task<IHausUdpSubscription> SubscribeAsync<T>(Action<T> handler);
    Task<IHausUdpSubscription> SubscribeAsync<T>(Func<T, Task> handler);
}

internal class HausUdpClient : IHausUdpClient
{
    private readonly UdpClient _client;
    private readonly IOptions<HausUdpSettings> _options;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ConcurrentDictionary<Guid, IHausUdpSubscription> _subscriptions;

    private int Port => _options.Value.Port;
    private bool HasStarted { get; set; }
    private Task ListeningTask { get; set; }

    public HausUdpClient(UdpClient client, IOptions<HausUdpSettings> options)
    {
        _client = client;
        _options = options;
        _cancellationTokenSource = new CancellationTokenSource();
        _subscriptions = new ConcurrentDictionary<Guid, IHausUdpSubscription>();
        HasStarted = false;
    }

    public Task BroadcastAsync<T>(T value)
    {
        var bytes = HausJsonSerializer.SerializeToBytes(value).ToArray();
        return _client.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, Port));
    }

    public Task<IHausUdpSubscription> SubscribeAsync<T>(Action<T> handler)
    {
        return SubscribeAsync<T>(value =>
        {
            handler.Invoke(value);
            return Task.CompletedTask;
        });
    }

    public Task<IHausUdpSubscription> SubscribeAsync<T>(Func<T, Task> handler)
    {
        EnsureListenerStarted();

        var subscription = new HausUdpSubscription<T>(handler, UnsubscribeAsync);
        _subscriptions.GetOrAdd(subscription.Id, subscription);
        return Task.FromResult<IHausUdpSubscription>(subscription);
    }

    private Task UnsubscribeAsync(IHausUdpSubscription subscription)
    {
        _subscriptions.TryRemove(subscription.Id, out subscription);
        return Task.CompletedTask;
    }

    private void EnsureListenerStarted()
    {
        if (HasStarted)
            return;

        HasStarted = true;
        ListeningTask = StartListening(_cancellationTokenSource.Token);
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

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        _client.Dispose();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (ListeningTask != null)
        {
            await ListeningTask;
            ListeningTask.Dispose();
        }
    }
}