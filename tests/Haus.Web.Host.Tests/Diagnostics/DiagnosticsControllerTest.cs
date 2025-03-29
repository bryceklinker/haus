using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Diagnostics;
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

        var model = HausModelFactory.MqttDiagnosticsMessageModel();
        var client = factory.CreateAuthenticatedClient();
        await client.ReplayDiagnosticsMessageAsync(model);

        Eventually.Assert(() =>
        {
            received?.Topic.Should().Be("my-topic");
            JObject.Parse(received.ConvertPayloadToString()).Value<int>("id").Should().Be(65);
        });
    }

    [Fact]
    public async Task WhenUnauthenticatedClientReplaysMessageThenRespondsWithUnauthorized()
    {
        var client = factory.CreateUnauthenticatedClient();
        var response = await client.ReplayDiagnosticsMessageAsync(HausModelFactory.MqttDiagnosticsMessageModel());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
