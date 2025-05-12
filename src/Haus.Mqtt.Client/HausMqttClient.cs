using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Mqtt.Client.Settings;
using Haus.Mqtt.Client.Subscriptions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Mqtt.Client;

public interface IHausMqttClient : IAsyncDisposable
{
    bool IsConnected { get; }
    bool IsStarted { get; }
    Task PingAsync(CancellationToken token);
    Task<IHausMqttSubscription> SubscribeAsync(string topic, Func<MqttApplicationMessage, Task> handler);
    Task<IHausMqttSubscription> SubscribeAsync(string topic, Action<MqttApplicationMessage> handler);
    Task PublishAsync(MqttApplicationMessage message);
    Task PublishAsync(string topic, object payload);
    Task PublishHausEventAsync<T>(IHausEventCreator<T> creator, string? topicName = null);
}

internal class HausMqttClient : IHausMqttClient
{
    private const string AllTopicsFilter = "#";
    private readonly IManagedMqttClient _mqttClient;
    private readonly IOptions<HausMqttSettings> _settings;
    private readonly Action? _onDisposed;
    private readonly Lazy<Task> _setupMqttListener;
    private readonly ConcurrentDictionary<Guid, IHausMqttSubscription> _subscriptions;

    private string EventsTopic => _settings.Value.EventsTopic;
    private string CommandsTopic => _settings.Value.CommandsTopic;
    public bool IsConnected => _mqttClient.IsConnected;
    public bool IsStarted => _mqttClient.IsStarted;

    public HausMqttClient(IManagedMqttClient mqttClient, IOptions<HausMqttSettings> settings, Action? onDisposed = null)
    {
        _mqttClient = mqttClient;
        _settings = settings;
        _onDisposed = onDisposed;
        _subscriptions = new ConcurrentDictionary<Guid, IHausMqttSubscription>();
        _setupMqttListener = new Lazy<Task>(SetupMqttListenerAsync);
    }

    public Task PingAsync(CancellationToken token)
    {
        return _mqttClient.PingAsync(token);
    }

    public async Task<IHausMqttSubscription> SubscribeAsync(string topic, Func<MqttApplicationMessage, Task> handler)
    {
        await _setupMqttListener.Value;
        var subscription = new HausMqttSubscription(topic, handler, Unsubscribe);
        _subscriptions.GetOrAdd(subscription.Id, _ => subscription);
        return subscription;
    }

    public Task<IHausMqttSubscription> SubscribeAsync(string topic, Action<MqttApplicationMessage> handler)
    {
        return SubscribeAsync(
            topic,
            msg =>
            {
                handler.Invoke(msg);
                return Task.CompletedTask;
            }
        );
    }

    public async Task PublishAsync(MqttApplicationMessage message)
    {
        await _mqttClient.EnqueueAsync(message);
    }

    public async Task PublishAsync(string topic, object payload)
    {
        await PublishAsync(
            new MqttApplicationMessage { Topic = topic, PayloadSegment = HausJsonSerializer.SerializeToBytes(payload) }
        );
    }

    public async Task PublishHausEventAsync<T>(IHausEventCreator<T> creator, string? topicName = null)
    {
        await PublishAsync(topicName ?? EventsTopic, creator.AsHausEvent());
    }

    public ValueTask DisposeAsync()
    {
        _mqttClient.Dispose();
        GC.SuppressFinalize(this);
        _onDisposed?.Invoke();
        return ValueTask.CompletedTask;
    }

    private async Task SetupMqttListenerAsync()
    {
        _mqttClient.ApplicationMessageReceivedAsync += MqttMessageHandler;
        await _mqttClient.SubscribeAsync(AllTopicsFilter);
    }

    private async Task MqttMessageHandler(MqttApplicationMessageReceivedEventArgs args)
    {
        await Task.WhenAll(_subscriptions.Values.Select(s => s.ExecuteAsync(args.ApplicationMessage)));
    }

    private Task Unsubscribe(IHausMqttSubscription subscription)
    {
        _subscriptions.TryRemove(subscription.Id, out _);
        return Task.CompletedTask;
    }
}
