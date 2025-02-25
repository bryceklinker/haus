using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Diagnostics;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Diagnostics;

[Collection(HausWebHostCollectionFixture.Name)]
public class DiagnosticsRealtimeApiTest
{
    private static readonly DateTime CurrentTime = new(2020, 3, 9, 2, 3, 4);
    private readonly HausWebHostApplicationFactory _factory;

    public DiagnosticsRealtimeApiTest(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _factory.SetClockTime(CurrentTime);
    }

    [Fact]
    public async Task WhenMqttMessagePublishedThenMessageIsRelayedToSignalr()
    {
        var connection = await _factory.CreateHubConnection("diagnostics");
        var mqttClient = await _factory.GetMqttClient();

        var mqttMessages = new ConcurrentBag<MqttDiagnosticsMessageModel>();
        connection.On<MqttDiagnosticsMessageModel>("OnMqttMessage", mqttMessages.Add);

        await mqttClient.PublishAsync("my-topic", "this is data");
        Eventually.Assert(() =>
        {
            mqttMessages
                .Should()
                .Contain(e => e.Topic == "my-topic")
                .And.Contain(e => e.Payload.ToString() == "this is data")
                .And.Contain(e => e.Timestamp == CurrentTime);
        });
    }
}
