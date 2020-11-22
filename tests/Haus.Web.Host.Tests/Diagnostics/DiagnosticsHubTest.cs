using System;
using System.Text.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Diagnostics;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;
using Xunit.Abstractions;

namespace Haus.Web.Host.Tests.Diagnostics
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DiagnosticsHubTest
    {
        private static readonly DateTime CurrentTime = new DateTime(2020, 3, 9, 2, 3, 4);
        private readonly HausWebHostApplicationFactory _factory;
        private ITestOutputHelper Output { get; }

        public DiagnosticsHubTest(HausWebHostApplicationFactory factory, ITestOutputHelper output)
        {
            _factory = factory;
            _factory.SetClockTime(CurrentTime);
            Output = output;
        }

        [Fact]
        public async Task WhenMqttMessagePublishedThenMessageIsRelayedToSignalr()
        {
            MqttDiagnosticsMessageModel received = null;
            var connection = await _factory.CreateHubConnection("diagnostics");
            var mqttClient = await _factory.GetMqttClient();
            
            connection.On<MqttDiagnosticsMessageModel>("OnMqttMessage", arg =>
            {
                received = arg;
            });

            await mqttClient.PublishAsync("my-topic", "this is data");
            await Eventually.Assert(() =>
            {
                Output.WriteLine($"Received is: {JsonSerializer.Serialize(received)}");
                Assert.True(Guid.TryParse(received.Id, out _));
                Assert.Equal("my-topic", received.Topic);
                Assert.Equal("this is data", received.Payload.ToString());
                Assert.Equal(CurrentTime, received.Timestamp);
            });
        }
    }
}