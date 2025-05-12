using System.Threading.Tasks;
using FluentAssertions;
using Haus.Mqtt.Client.Tests.Support;
using Xunit;

namespace Haus.Mqtt.Client.Tests;

public class HausMqttClientFactoryTests
{
    private const string DEFAULT_MQTT_URL = "mqtt://localhost:1883";
    private readonly IHausMqttClientFactory _hausClientFactory = new SupportFactory().CreateFactory();

    [Fact]
    public async Task WhenClientCreatedThenClientIsStarted()
    {
        var client = await _hausClientFactory.CreateClient();

        client.IsStarted.Should().BeTrue();
    }

    [Fact]
    public async Task WhenClientCreatedMultipleTimesThenReturnsTheSameClient()
    {
        var first = await _hausClientFactory.CreateClient();
        var second = await _hausClientFactory.CreateClient();

        first.Should().BeSameAs(second);
    }

    [Fact]
    public async Task WhenClientCreatedThenClientIsConnected()
    {
        var client = await _hausClientFactory.CreateClient();

        client.IsConnected.Should().BeTrue();
    }

    [Fact]
    public async Task WhenClientCratedForASpecificUrlThenReturnsANewClient()
    {
        var standardUrlClient = await _hausClientFactory.CreateClient();
        var otherUrlClient = await _hausClientFactory.CreateClient("mqtt://127.0.0.1:1883");

        standardUrlClient.Should().NotBeSameAs(otherUrlClient);
    }

    [Fact]
    public async Task WhenClientCreatedForSpecificUrlMultipleTimesThenReturnsTheSameClient()
    {
        var first = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);
        var second = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);

        first.Should().BeSameAs(second);
    }

    [Fact]
    public async Task WhenClientCreatedAfterDisposingThenClientIsRecreated()
    {
        var first = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);
        await first.DisposeAsync();

        var second = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);
        first.Should().NotBeSameAs(second);
    }
}
