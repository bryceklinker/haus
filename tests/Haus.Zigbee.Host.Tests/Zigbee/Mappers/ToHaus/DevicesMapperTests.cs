using System.Linq;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Resolvers;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus;

public class GetDevicesMapperTests
{
    private readonly DevicesMapper _mapper;

    public GetDevicesMapperTests()
    {
        var zigbeeOptions = OptionsFactory.CreateZigbeeOptions();
        var hausOptions = OptionsFactory.CreateHausOptions();
        _mapper = new DevicesMapper(zigbeeOptions, hausOptions, new DeviceTypeResolver(hausOptions));
    }

    [Fact]
    public void WhenTopicIsConfigDevicesThenIsSupported()
    {
        var message = new Zigbee2MqttMessageBuilder().WithDevicesTopic().BuildZigbee2MqttMessage();

        Assert.True(_mapper.IsSupported(message));
    }

    [Fact]
    public void WhenTopicIsNotConfigDevicesThenUnsupported()
    {
        var message = new Zigbee2MqttMessageBuilder().WithTopicPath("idk").BuildZigbee2MqttMessage();

        Assert.False(_mapper.IsSupported(message));
    }

    [Fact]
    public void WhenOneDeviceIsInGetDevicesMessageThenReturnsOneDeviceDiscoveredMessage()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDevicesTopic()
            .WithDeviceInPayload(device =>
            {
                device.Add("friendly_name", "boom");
            })
            .BuildZigbee2MqttMessage();

        var result = _mapper.Map(message).ToArray();

        Assert.Single(result);
        Assert.Equal(Defaults.HausOptions.EventsTopic, result.Single().Topic);
    }

    [Fact]
    public void WhenOneDeviceIsInGetDeviceMessagesThen()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDevicesTopic()
            .WithDeviceInPayload(device =>
            {
                device.Add("friendly_name", "hello");
                device.Add("description", "my desc");
                device.Add("model", "65");
                device.Add("vendor", "76");
                device.Add("powerSource", "Battery");
            })
            .BuildZigbee2MqttMessage();

        var result = _mapper.Map(message).Single();

        var @event = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredEvent>>(result.PayloadSegment);
        Assert.Equal(DeviceDiscoveredEvent.Type, @event?.Type);
        Assert.Equal("hello", @event?.Payload?.Id);
        Assert.Contains(new MetadataModel("model", "65"), @event?.Payload?.Metadata!);
        Assert.Contains(new MetadataModel("vendor", "76"), @event?.Payload?.Metadata!);
        Assert.Contains(new MetadataModel("description", "my desc"), @event?.Payload?.Metadata!);
        Assert.Contains(new MetadataModel("powerSource", "Battery"), @event?.Payload?.Metadata!);
    }

    [Fact]
    public void WhenDeviceIsMappedThenDeviceTypeIsResolved()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDevicesTopic()
            .WithDeviceInPayload(device =>
            {
                device.Add("model", "929002335001");
                device.Add("vendor", "Philips");
            })
            .BuildZigbee2MqttMessage();

        var result = _mapper.Map(message).Single();

        var @event = HausJsonSerializer.Deserialize<HausEvent<DeviceDiscoveredEvent>>(result.PayloadSegment);
        Assert.Equal(DeviceType.Light, @event?.Payload?.DeviceType);
    }

    [Fact]
    public void WhenMultipleDevicesAreInMessageThenReturnsMultipleDiscoveredEvents()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDevicesTopic()
            .WithDeviceInPayload()
            .WithDeviceInPayload()
            .WithDeviceInPayload()
            .BuildZigbee2MqttMessage();

        var result = _mapper.Map(message);

        Assert.Equal(3, result.Count());
    }
}
