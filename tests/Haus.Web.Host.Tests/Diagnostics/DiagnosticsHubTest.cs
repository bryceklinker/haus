using System.Threading.Tasks;
using Haus.Web.Host.Diagnostics.Models;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Diagnostics
{
    public class DiagnosticsHubTest : IClassFixture<HausWebHostApplicationFactory>
    {
        private readonly HausWebHostApplicationFactory _factory;

        public DiagnosticsHubTest(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
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
                Assert.Equal("my-topic", received.Topic);
                Assert.Equal("this is data", received.Payload.ToString());
            });
        }
    }
}