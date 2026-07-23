using System.Threading.Tasks;
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

        Assert.True(client.IsStarted);
    }

    [Fact]
    public async Task WhenClientCreatedMultipleTimesThenReturnsTheSameClient()
    {
        var first = await _hausClientFactory.CreateClient();
        var second = await _hausClientFactory.CreateClient();

        Assert.Same(second, first);
    }

    [Fact]
    public async Task WhenClientCreatedThenClientIsConnected()
    {
        var client = await _hausClientFactory.CreateClient();

        Assert.True(client.IsConnected);
    }

    [Fact]
    public async Task WhenClientCratedForASpecificUrlThenReturnsANewClient()
    {
        var standardUrlClient = await _hausClientFactory.CreateClient();
        var otherUrlClient = await _hausClientFactory.CreateClient("mqtt://127.0.0.1:1883");

        Assert.NotSame(otherUrlClient, standardUrlClient);
    }

    [Fact]
    public async Task WhenClientCreatedForSpecificUrlMultipleTimesThenReturnsTheSameClient()
    {
        var first = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);
        var second = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);

        Assert.Same(second, first);
    }

    [Fact]
    public async Task WhenClientCreatedAfterDisposingThenClientIsRecreated()
    {
        var first = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);
        await first.DisposeAsync();

        var second = await _hausClientFactory.CreateClient(DEFAULT_MQTT_URL);
        Assert.NotSame(second, first);
    }
}
