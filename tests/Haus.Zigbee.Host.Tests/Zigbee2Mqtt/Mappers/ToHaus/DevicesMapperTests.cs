using System.Linq;
using FluentAssertions;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
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
        var message = new Zigbee2MqttMessageBuilder()
            .WithDevicesTopic()
            .BuildZigbee2MqttMessage();

        _mapper.IsSupported(message).Should().BeTrue();
    }

    [Fact]
    public void WhenTopicIsNotConfigDevicesThenUnsupported()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithTopicPath("idk")
            .BuildZigbee2MqttMessage();

        _mapper.IsSupported(message).Should().BeFalse();
    }

    [Fact]
    public void WhenOneDeviceIsInGetDevicesMessageThenReturnsOneDeviceDiscoveredMessage()
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithDevicesTopic()
            .WithDeviceInPayload(device => { device.Add("friendly_name", "boom"); })
            .BuildZigbee2MqttMessage();

        var result = _mapper.Map(message).ToArray();

        result.Should().ContainSingle();
        result.Single().Topic.Should().Be(Defaults.HausOptions.EventsTopic);
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
        @event.Type.Should().Be(DeviceDiscoveredEvent.Type);
        @event.Payload.Id.Should().Be("hello");
        @event.Payload.Metadata.Should()
            .ContainEquivalentOf(new MetadataModel("model", "65"))
            .And.ContainEquivalentOf(new MetadataModel("vendor", "76"))
            .And.ContainEquivalentOf(new MetadataModel("description", "my desc"))
            .And.ContainEquivalentOf(new MetadataModel("powerSource", "Battery"));
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
        @event.Payload.DeviceType.Should().Be(DeviceType.Light);
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

        result.Should().HaveCount(3);
    }
}