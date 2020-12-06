using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Common.Mqtt
{
    public interface IHausMqttClient : IAsyncDisposable
    {
        Task<IHausMqttSubscription> SubscribeAsync(string topic, Func<MqttApplicationMessage, Task> handler);
        Task PublishAsync(MqttApplicationMessage message);
    }

    public class HausMqttClient : IHausMqttClient
    {
        private const string AllTopicsFilter = "#";
        private readonly IManagedMqttClient _mqttClient;
        private readonly Lazy<Task> _setupMqttListener;
        private readonly ConcurrentDictionary<Guid, IHausMqttSubscription> _subscriptions;
        private readonly ILogger<HausMqttClient> _logger;

        public HausMqttClient(IManagedMqttClient mqttClient, ILoggerFactory loggerFactory)
        {
            _mqttClient = mqttClient;
            _subscriptions = new ConcurrentDictionary<Guid, IHausMqttSubscription>();
            _setupMqttListener = new Lazy<Task>(SetupMqttListenerAsync);
            _logger = loggerFactory.CreateLogger<HausMqttClient>();
        }

        public async Task<IHausMqttSubscription> SubscribeAsync(string topic, Func<MqttApplicationMessage, Task> handler)
        {
            await _setupMqttListener.Value;
            var subscription = new HausMqttSubscription(topic, handler, Unsubscribe);
            _subscriptions.GetOrAdd(subscription.Id, (id) => subscription);
            return subscription;
        }

        public async Task PublishAsync(MqttApplicationMessage message)
        {
            await _mqttClient.PublishAsync(message);
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