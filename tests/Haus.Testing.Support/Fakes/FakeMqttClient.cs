using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Internal;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;

namespace Haus.Testing.Support.Fakes;

public class FakeMqttClient : IManagedMqttClient, IMqttClient
{
    private readonly List<MqttApplicationMessage> _publishedMessages = new();
    private readonly MqttClientEvents _events = new();

    public bool IsDisposed { get; private set; }
    public bool IsStarted { get; private set; }

    private bool IsConnected { get; set; }
    bool IManagedMqttClient.IsConnected => IsConnected;
    bool IMqttClient.IsConnected => IsConnected;
    private MqttClientOptions Options { get; set; }
    
    event Func<MqttApplicationMessageReceivedEventArgs, Task> IMqttClient.ApplicationMessageReceivedAsync
    {
        add => _events.ApplicationMessageReceivedEvent.AddHandler(value);
        remove => _events.ApplicationMessageReceivedEvent.RemoveHandler(value);
    }

    event Func<MqttClientConnectedEventArgs, Task> IMqttClient.ConnectedAsync
    {
        add => _events.ConnectedEvent.AddHandler(value);
        remove => _events.ConnectedEvent.RemoveHandler(value);
    }

    public event Func<MqttClientConnectingEventArgs, Task> ConnectingAsync;

    event Func<MqttClientDisconnectedEventArgs, Task> IMqttClient.DisconnectedAsync
    {
        add => _events.DisconnectedEvent.AddHandler(value);
        remove => _events.DisconnectedEvent.RemoveHandler(value);
    }

    public event Func<InspectMqttPacketEventArgs, Task> InspectPacketAsync;
    MqttClientOptions IMqttClient.Options => Options;

    ManagedMqttClientOptions IManagedMqttClient.Options => new()
    {
        ClientOptions = Options
    };

    public IMqttClient InternalClient => this;

    public int PendingApplicationMessagesCount { get; } = 0;
    public event Func<ApplicationMessageProcessedEventArgs, Task> ApplicationMessageProcessedAsync;

    event Func<MqttApplicationMessageReceivedEventArgs, Task> IManagedMqttClient.ApplicationMessageReceivedAsync
    {
        add => _events.ApplicationMessageReceivedEvent.AddHandler(value);
        remove => _events.ApplicationMessageReceivedEvent.RemoveHandler(value);
    }

    public event Func<ApplicationMessageSkippedEventArgs, Task> ApplicationMessageSkippedAsync;

    event Func<MqttClientConnectedEventArgs, Task> IManagedMqttClient.ConnectedAsync
    {
        add => _events.ConnectedEvent.AddHandler(value);
        remove => _events.ConnectedEvent.RemoveHandler(value);
    }

    public event Func<ConnectingFailedEventArgs, Task> ConnectingFailedAsync;
    public event Func<EventArgs, Task> ConnectionStateChangedAsync;

    event Func<MqttClientDisconnectedEventArgs, Task> IManagedMqttClient.DisconnectedAsync
    {
        add => _events.DisconnectedEvent.AddHandler(value);
        remove => _events.DisconnectedEvent.RemoveHandler(value);
    }

    public event Func<ManagedProcessFailedEventArgs, Task> SynchronizingSubscriptionsFailedAsync;
    public event Func<SubscriptionsChangedEventArgs, Task> SubscriptionsChangedAsync;

    public MqttApplicationMessage[] PublishedMessages => _publishedMessages.ToArray();

    public Exception PingException { get; set; }
    public Exception ConnectException { get; set; }
    public void Dispose()
    {
        IsDisposed = true;
    }

    public Task StartAsync(ManagedMqttClientOptions options)
    {
        IsStarted = true;
        Options = options.ClientOptions;
        return Task.CompletedTask;
    }

    public Task StopAsync(bool cleanDisconnect = true)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        IsStarted = false;
        return Task.CompletedTask;
    }

    public Task<MqttClientConnectResult> ConnectAsync(MqttClientOptions options, CancellationToken cancellationToken)
    {
        if (ConnectException != null)
            throw ConnectException;

        Options = options;
        IsConnected = true;
        return Task.FromResult(new MqttClientConnectResult());
    }

    public Task DisconnectAsync(MqttClientDisconnectOptions options, CancellationToken cancellationToken)
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public async Task EnqueueAsync(MqttApplicationMessage applicationMessage)
    {
        _publishedMessages.Add(applicationMessage);
        await _events.ApplicationMessageReceivedEvent.InvokeAsync(
            new MqttApplicationMessageReceivedEventArgs("", applicationMessage, new MqttPublishPacket(),
                (_, _) => Task.CompletedTask)
        ).ConfigureAwait(false);
    }

    public async Task EnqueueAsync(ManagedMqttApplicationMessage applicationMessage)
    {
        await EnqueueAsync(applicationMessage.ApplicationMessage);
    }

    Task IMqttClient.PingAsync(CancellationToken cancellationToken)
    {
        if (PingException == null)
            return Task.CompletedTask;

        throw PingException;
    }

    public async Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage applicationMessage,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await EnqueueAsync(applicationMessage);
        return new MqttClientPublishResult(0, MqttClientPublishReasonCode.Success, "", Array.Empty<MqttUserProperty>());
    }

    public Task SendExtendedAuthenticationExchangeDataAsync(MqttExtendedAuthenticationExchangeData data,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<MqttClientSubscribeResult> SubscribeAsync(MqttClientSubscribeOptions options,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new MqttClientSubscribeResult(
            0,
            Array.Empty<MqttClientSubscribeResultItem>(),
            "",
            Array.Empty<MqttUserProperty>()
        ));
    }

    public Task<MqttClientUnsubscribeResult> UnsubscribeAsync(MqttClientUnsubscribeOptions options,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new MqttClientUnsubscribeResult(
                0,
                Array.Empty<MqttClientUnsubscribeResultItem>(),
                "",
                Array.Empty<MqttUserProperty>()
            )
        );
    }

    Task IManagedMqttClient.PingAsync(CancellationToken cancellationToken)
    {
        if (PingException == null)
            return Task.CompletedTask;
        throw PingException;
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