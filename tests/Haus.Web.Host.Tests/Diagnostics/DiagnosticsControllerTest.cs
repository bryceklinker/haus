using System.Net;
using System.Threading.Tasks;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using MQTTnet;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Web.Host.Tests.Diagnostics;

[Collection(HausWebHostCollectionFixture.Name)]
public class DiagnosticsControllerTest(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenMessageIsReplayedThenMessageIsSentToMqttTopic()
    {
        var mqtt = await factory.GetMqttClient();
        MqttApplicationMessage? received = null;
        await mqtt.SubscribeAsync("my-topic", msg => received = msg);

        var model = HausModelFactory.MqttDiagnosticsMessageModel() with
        {
            Topic = "my-topic",
            Payload = new { id = 65 },
        };
        var client = factory.CreateAuthenticatedClient();
        await client.ReplayDiagnosticsMessageAsync(model);

        Eventually.Assert(() =>
        {
            if (received != null)
            {
                Assert.Equal("my-topic", received.Topic);
            }
            Assert.Equal(65, JObject.Parse(received.ConvertPayloadToString()).Value<int>("id"));
        });
    }

    [Fact]
    public async Task WhenUnauthenticatedClientReplaysMessageThenRespondsWithUnauthorized()
    {
        var client = factory.CreateUnauthenticatedClient();
        var response = await client.ReplayDiagnosticsMessageAsync(HausModelFactory.MqttDiagnosticsMessageModel());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
