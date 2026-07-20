using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
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
            var filteredMessages = mqttMessages.Where(e => e.Payload != null);
            Assert.Contains(filteredMessages, e => e.Topic == "my-topic");
            Assert.Contains(filteredMessages, e => e.Payload!.ToString() == "this is data");
            Assert.Contains(filteredMessages, e => e.Timestamp == CurrentTime);
        });
    }
}
