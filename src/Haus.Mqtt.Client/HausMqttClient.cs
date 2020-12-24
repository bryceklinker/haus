using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Mqtt.Client.Settings;
using Haus.Mqtt.Client.Subscriptions;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Mqtt.Client
{
    public interface IHausMqttClient : IAsyncDisposable
    {
        Task<IHausMqttSubscription> SubscribeAsync(string topic, Func<MqttApplicationMessage, Task> handler);
        Task<IHausMqttSubscription> SubscribeAsync(string topic, Action<MqttApplicationMessage> handler);
        Task PublishAsync(MqttApplicationMessage message);
        Task PublishAsync(string topic, object payload);
        Task PublishHausEventAsync<T>(IHausEventCreator<T> creator, string topicName = null);
        Task PublishHausCommandAsync<T>(IHausCommandCreator<T> creator, string topicName = null);
    }

    public class HausMqttClient : IHausMqttClient
    {
        private const string AllTopicsFilter = "#";
        private readonly IManagedMqttClient _mqttClient;
        private readonly IOptions<HausMqttSettings> _settings;
        private readonly Lazy<Task> _setupMqttListener;
        private readonly ConcurrentDictionary<Guid, IHausMqttSubscription> _subscriptions;

        private string EventsTopic => _settings.Value.EventsTopic;
        private string CommandsTopic => _settings.Value.CommandsTopic;
        
        public HausMqttClient(IManagedMqttClient mqttClient, IOptions<HausMqttSettings> settings)
        {
            _mqttClient = mqttClient;
            _settings = settings;
            _subscriptions = new ConcurrentDictionary<Guid, IHausMqttSubscription>();
            _setupMqttListener = new Lazy<Task>(SetupMqttListenerAsync);
        }

        public async Task<IHausMqttSubscription> SubscribeAsync(string topic, Func<MqttApplicationMessage, Task> handler)
        {
            await _setupMqttListener.Value;
            var subscription = new HausMqttSubscription(topic, handler, Unsubscribe);
            _subscriptions.GetOrAdd(subscription.Id, (id) => subscription);
            return subscription;
        }

        public Task<IHausMqttSubscription> SubscribeAsync(string topic, Action<MqttApplicationMessage> handler)
        {
            return SubscribeAsync(topic, msg =>
            {
                handler.Invoke(msg);
                return Task.CompletedTask;
            });
        }

        public Task PublishAsync(MqttApplicationMessage message)
        {
            return _mqttClient.PublishAsync(message);
        }

        public Task PublishAsync(string topic, object payload)
        {
            return PublishAsync(new MqttApplicationMessage
            {
                Topic = topic,
                Payload = HausJsonSerializer.SerializeToBytes(payload)
            });
        }

        public Task PublishHausEventAsync<T>(IHausEventCreator<T> creator, string topicName = null)
        {
            return PublishAsync(topicName ?? EventsTopic, creator.AsHausEvent());
        }

        public Task PublishHausCommandAsync<T>(IHausCommandCreator<T> creator, string topicName = null)
        {
            return PublishAsync(topicName ?? CommandsTopic, creator.AsHausCommand());
        }

        public ValueTask DisposeAsync()
        {
            _mqttClient.Dispose();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

        private async Task SetupMqttListenerAsync()
        {
            await _mqttClient.SubscribeAsync(AllTopicsFilter);
            _mqttClient.UseApplicationMessageReceivedHandler(MqttMessageHandler);
        }

        private async Task MqttMessageHandler(MqttApplicationMessageReceivedEventArgs args)
        {
            await Task.WhenAll(_subscriptions.Values.Select(s => s.ExecuteAsync(args.ApplicationMessage)));
        }

        private Task Unsubscribe(IHausMqttSubscription subscription)
        {
            _subscriptions.TryRemove(subscription.Id, out subscription);
            return Task.CompletedTask;
        }
    }
}