using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Events;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee.Events;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Mqtt.Client.Wrappers;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Web.Host.Zigbee;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Web.Host.Tests.Zigbee;

public class ZigbeeKnownDeviceSeedPublisherTests : IAsyncLifetime
{
    private readonly FakeMqttClientFactory _fakeMqttClientFactory = new();
    private IHausMqttClient? _mqttClient;
    private ZigbeeKnownDeviceSeedPublisher? _publisher;

    public async Task InitializeAsync()
    {
        var provider = new ServiceCollection()
            .AddLogging()
            .AddHausMqtt()
            .Replace<IMqttFactory>(_fakeMqttClientFactory)
            .Configure<HausMqttSettings>(opts => opts.Server = "mqtt://localhost:1883")
            .BuildServiceProvider();

        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _mqttClient = await mqttClientFactory.CreateClient();
        _publisher = new ZigbeeKnownDeviceSeedPublisher(
            mqttClientFactory,
            provider.GetRequiredService<IOptions<HausMqttSettings>>()
        );
    }

    public async Task DisposeAsync()
    {
        await _mqttClient!.DisposeAsync();
    }

    [Fact]
    public async Task WhenHandledThenPublishesRetainedMessageOnTheSeedTopic()
    {
        var entry = new ZigbeeKnownDeviceSeedEntry("device-1", 4660, [new DeviceEndpointModel(1, [6, 8])], true);
        var notification = new ZigbeeKnownDeviceSeedRequestedEvent(new ZigbeeKnownDeviceSeedEvent([entry]));

        await _publisher!.Handle(notification, CancellationToken.None);

        var message = Assert.Single(_fakeMqttClientFactory.Client.PublishedMessages);
        Assert.Equal(DefaultHausMqttTopics.ZigbeeKnownDeviceSeedTopic, message.Topic);
        Assert.True(message.Retain);
        HausJsonSerializer.TryDeserialize(message.PayloadSegment, out HausEvent<ZigbeeKnownDeviceSeedEvent>? evt);
        var published = Assert.Single(evt!.Payload.Devices);
        Assert.Equal("device-1", published.ExternalId);
        Assert.Equal((ushort)4660, published.NetworkAddress);
        Assert.True(published.IsSleepy);
    }

    [Fact]
    public async Task WhenNoDevicesInSeedThenPublishesEmptyRetainedSnapshot()
    {
        var notification = new ZigbeeKnownDeviceSeedRequestedEvent(new ZigbeeKnownDeviceSeedEvent([]));

        await _publisher!.Handle(notification, CancellationToken.None);

        var message = Assert.Single(_fakeMqttClientFactory.Client.PublishedMessages);
        Assert.True(message.Retain);
        HausJsonSerializer.TryDeserialize(message.PayloadSegment, out HausEvent<ZigbeeKnownDeviceSeedEvent>? evt);
        Assert.Empty(evt!.Payload.Devices);
    }
}
