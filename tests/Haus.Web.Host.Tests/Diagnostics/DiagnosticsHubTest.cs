using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Diagnostics;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Diagnostics
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DiagnosticsHubTest
    {
        private static readonly DateTime CurrentTime = new DateTime(2020, 3, 9, 2, 3, 4);
        private readonly HausWebHostApplicationFactory _factory;

        public DiagnosticsHubTest(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _factory.SetClockTime(CurrentTime);
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
            Eventually.Assert(() =>
            {
                received.Id.Should().BeAGuid();
                received.Topic.Should().Be("my-topic");
                received.Payload.Should().Be("this is data");
                received.Timestamp.Should().Be(CurrentTime);
            });
        }
    }
}