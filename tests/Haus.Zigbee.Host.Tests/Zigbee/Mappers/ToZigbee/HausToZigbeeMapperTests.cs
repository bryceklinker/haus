using System.Linq;
using System.Text;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToZigbee;

public class HausToZigbeeMapperTests
{
    private const string Zigbee2MqttBaseTopic = "something";
    private readonly HausToZigbeeMapper _mapper;

    public HausToZigbeeMapperTests()
    {
        var config = ConfigurationFactory.CreateConfig(Zigbee2MqttBaseTopic);
        var mappers = ServiceProviderFactory.Create(config).GetServices<IToZigbeeMapper>();

        _mapper = new HausToZigbeeMapper(mappers);
    }

    [Fact]
    public void WhenStartDiscoveryCommandReceivedThenReturnsNoMessages()
    {
        // Discovery handling moved off this MQTT-relay aggregator onto HausDiscoveryToZigbeeMapper,
        // wired directly into the coordinator-driven relay -- nothing here claims this message.
        var original = new StartDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");

        var result = _mapper.Map(original);

        Assert.Empty(result);
    }

    [Fact]
    public void WhenStopDiscoveryCommandReceivedThenReturnsNoMessages()
    {
        var original = new StopDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");

        var result = _mapper.Map(original);

        Assert.Empty(result);
    }

    [Fact]
    public void WhenSyncDevicesCommandReceivedThenReturnsNoMessages()
    {
        var original = new SyncDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");

        var result = _mapper.Map(original);

        Assert.Empty(result);
    }

    [Fact]
    public void WhenDeviceLightingCommandReceivedThenReturnsSetDeviceMessage()
    {
        var original = new DeviceLightingChangedEvent(new DeviceModel { ExternalId = "my-ext-id" }, new LightingModel())
            .AsHausCommand()
            .ToMqttMessage("haus/commands");

        var result = _mapper.Map(original).Single();

        var payload = JObject.Parse(Encoding.UTF8.GetString(result.PayloadSegment));
        Assert.Equal($"{Zigbee2MqttBaseTopic}/my-ext-id/set", result.Topic);
        Assert.Equal("OFF", payload.Value<string>("state"));
    }
}
