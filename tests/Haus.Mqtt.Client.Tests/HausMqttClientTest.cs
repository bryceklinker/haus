using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Mqtt.Client.Tests.Support;
using Haus.Testing.Support.Fakes;
using MQTTnet;
using Xunit;

namespace Haus.Mqtt.Client.Tests;

public class HausMqttClientTest : IAsyncLifetime
{
    private FakeMqttClient? _fakeMqttClient;
    private IHausMqttClient? _client;

    private IHausMqttClient Client
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_client);
            return _client;
        }
    }

    private FakeMqttClient FakeMqttClient
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_fakeMqttClient);
            return _fakeMqttClient;
        }
    }

    public async Task InitializeAsync()
    {
        var supportFactory = new SupportFactory();
        _fakeMqttClient = supportFactory.FakeClient;

        _client = await supportFactory.CreateClient();
    }

    [Fact]
    public async Task WhenSubscribingToTopicThenPublishedMessageGoesToSubscriber()
    {
        MqttApplicationMessage? actual = null;
        await Client.SubscribeAsync(
            "bob",
            msg =>
            {
                actual = msg;
                return Task.CompletedTask;
            }
        );

        var expected = new MqttApplicationMessage { Topic = "bob" };
        await FakeMqttClient.EnqueueAsync(expected);

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task WhenMultipleSubscribersThenPublishedMessagesGoToAllSubscribers()
    {
        var actuals = new List<MqttApplicationMessage>();
        await Client.SubscribeAsync(
            "#",
            msg =>
            {
                actuals.Add(msg);
                return Task.CompletedTask;
            }
        );
        await Client.SubscribeAsync(
            "#",
            msg =>
            {
                actuals.Add(msg);
                return Task.CompletedTask;
            }
        );

        await FakeMqttClient.EnqueueAsync(new MqttApplicationMessage());
        actuals.Should().HaveCount(2);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
