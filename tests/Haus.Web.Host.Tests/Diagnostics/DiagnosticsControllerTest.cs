using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Diagnostics;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using MQTTnet;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Web.Host.Tests.Diagnostics
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DiagnosticsControllerTest
    {
        private readonly HausWebHostApplicationFactory _factory;

        public DiagnosticsControllerTest(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenMessageIsReplayedThenMessageIsSentToMqttTopic()
        {
            var mqtt = await _factory.GetMqttClient();
            MqttApplicationMessage received = null;
            await mqtt.SubscribeAsync("my-topic", msg => received = msg);
                
            var model = new MqttDiagnosticsMessageModel
            {
                Payload = new {id = 65},
                Topic = "my-topic"
            };
            var client = _factory.CreateAuthenticatedClient();
            await client.ReplayDiagnosticsMessageAsync(model);

            Eventually.Assert(() =>
            {
                received.Topic.Should().Be("my-topic");
                JObject.Parse(received.ConvertPayloadToString()).Value<int>("id").Should().Be(65);
            });
        }

        [Fact]
        public async Task WhenUnauthenticatedClientReplaysMessageThenRespondsWithUnauthorized()
        {
            var client = _factory.CreateUnauthenticatedClient();
            var response = await client.ReplayDiagnosticsMessageAsync(new MqttDiagnosticsMessageModel());

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}