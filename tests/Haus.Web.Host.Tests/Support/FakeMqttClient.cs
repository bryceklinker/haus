using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.ExtendedAuthenticationExchange;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Receiving;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Tests.Support
{
    public class FakeMqttClient : IManagedMqttClient, IMqttClient
    {
        public bool IsDisposed { get; private set; }
        public bool IsStarted { get; private set; }
        public IMqttApplicationMessageReceivedHandler ApplicationMessageReceivedHandler { get; set; }

        private bool IsConnected { get; set; }
        bool IManagedMqttClient.IsConnected => IsConnected;
        bool IMqttClient.IsConnected => IsConnected;

        private IMqttClientOptions Options { get; set; }
        IMqttClientOptions IMqttClient.Options => Options;
        IManagedMqttClientOptions IManagedMqttClient.Options => new ManagedMqttClientOptions
        {
            ClientOptions = Options
        };

        public IMqttClient InternalClient => this;

        public int PendingApplicationMessagesCount { get; private set; }
        
        private IMqttClientConnectedHandler ConnectedHandler { get; set; }
        IMqttClientConnectedHandler IMqttClient.ConnectedHandler
        {
            get => ConnectedHandler;
            set => ConnectedHandler = value;
        }
        IMqttClientConnectedHandler IManagedMqttClient.ConnectedHandler 
        {
            get => ConnectedHandler;
            set => ConnectedHandler = value;
        }
        
        private IMqttClientDisconnectedHandler DisconnectedHandler { get; set; }
        IMqttClientDisconnectedHandler IMqttClient.DisconnectedHandler
        {
            get => DisconnectedHandler;
            set => DisconnectedHandler = value;
        }

        IMqttClientDisconnectedHandler IManagedMqttClient.DisconnectedHandler
        {
            get => DisconnectedHandler;
            set => DisconnectedHandler = value;
        }

        public IApplicationMessageProcessedHandler ApplicationMessageProcessedHandler { get; set; }
        public IApplicationMessageSkippedHandler ApplicationMessageSkippedHandler { get; set; }
        public IConnectingFailedHandler ConnectingFailedHandler { get; set; }
        public ISynchronizingSubscriptionsFailedHandler SynchronizingSubscriptionsFailedHandler { get; set; }

        public FakeMqttClient()
        {
            
        }
        
        public async Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage applicationMessage, CancellationToken cancellationToken)
        {
            await ApplicationMessageReceivedHandler.HandleApplicationMessageReceivedAsync(
                new MqttApplicationMessageReceivedEventArgs("", applicationMessage));
            return new MqttClientPublishResult();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public Task StartAsync(IManagedMqttClientOptions options)
        {
            IsStarted = true;
            Options = options.ClientOptions;
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            IsStarted = false;
            return Task.CompletedTask;
        }

        public Task<MqttClientAuthenticateResult> ConnectAsync(IMqttClientOptions options, CancellationToken cancellationToken)
        {
            Options = options;
            IsConnected = true;
            return Task.FromResult(new MqttClientAuthenticateResult());
        }

        public Task DisconnectAsync(MqttClientDisconnectOptions options, CancellationToken cancellationToken)
        {
            IsConnected = false;
            return Task.CompletedTask;
        }

        Task IMqttClient.PingAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SendExtendedAuthenticationExchangeDataAsync(MqttExtendedAuthenticationExchangeData data,
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<MqttClientSubscribeResult> SubscribeAsync(MqttClientSubscribeOptions options, CancellationToken cancellationToken)
        {
            return Task.FromResult(new MqttClientSubscribeResult());
        }

        public Task<MqttClientUnsubscribeResult> UnsubscribeAsync(MqttClientUnsubscribeOptions options, CancellationToken cancellationToken)
        {
            return Task.FromResult(new MqttClientUnsubscribeResult());
        }
        
        Task IManagedMqttClient.PingAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SubscribeAsync(IEnumerable<MqttTopicFilter> topicFilters)
        {
            return SubscribeAsync(new MqttClientSubscribeOptions
            {
                TopicFilters = topicFilters.ToList()
            }, CancellationToken.None);
        }

        public Task UnsubscribeAsync(IEnumerable<string> topics)
        {
            return UnsubscribeAsync(new MqttClientUnsubscribeOptions
            {
                TopicFilters = topics.ToList()
            }, CancellationToken.None);
        }

        public async Task PublishAsync(ManagedMqttApplicationMessage applicationMessages)
        {
            await PublishAsync(applicationMessages.ApplicationMessage, CancellationToken.None);
        }

    }
}