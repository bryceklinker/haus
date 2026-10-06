using System;
using Haus.Core.Common.Events;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Core.Models.Zigbee.Events;
using Xunit;

namespace Haus.Core.Tests.Common.Events;

public class RoutableHausEventFactoryTest
{
    private readonly RoutableEventFactory _factory = new();

    [Fact]
    public void WhenDeviceDiscoveredEventThenReturnsRoutableEventFromDeviceDiscovered()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new DeviceDiscoveredEvent($"{Guid.NewGuid()}").AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<DeviceDiscoveredEvent>>(routableEvent);
    }

    [Fact]
    public void WhenMultiSensorChangedThenReturnsRoutableEventFromMultiSensorChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new MultiSensorChanged($"{Guid.NewGuid()}").AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<MultiSensorChanged>>(routableEvent);
    }

    [Fact]
    public void WhenMotionSensorChangedThenReturnsRoutableEventFromMotionSensorChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new OccupancyChangedModel($"{Guid.NewGuid()}").AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<OccupancyChangedModel>>(routableEvent);
    }

    [Fact]
    public void WhenIlluminanceChangedThenReturnsRoutableEventFromIlluminanceChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(
            new IlluminanceChangedModel($"{Guid.NewGuid()}", 1, 1).AsHausEvent()
        );

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<IlluminanceChangedModel>>(routableEvent);
    }

    [Fact]
    public void WhenTemperatureSensorChangedThenReturnsRoutableEventFromTemperatureSensorChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(
            new TemperatureChangedModel($"{Guid.NewGuid()}", 21.5).AsHausEvent()
        );

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<TemperatureChangedModel>>(routableEvent);
    }

    [Fact]
    public void WhenBatteryChangedThenReturnsRoutableEventFromBatteryChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new BatteryChangedModel($"{Guid.NewGuid()}", 80).AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<BatteryChangedModel>>(routableEvent);
    }

    [Fact]
    public void WhenZigbeeConnectionStatusChangedThenReturnsRoutableEventFromZigbeeConnectionStatusChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(
            new ZigbeeConnectionStatusChangedEvent(true, null, null).AsHausEvent()
        );

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<ZigbeeConnectionStatusChangedEvent>>(routableEvent);
    }

    [Fact]
    public void WhenZigbeeDeviceJoinedThenReturnsRoutableEventFromZigbeeDeviceJoined()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new ZigbeeDeviceJoinedEvent("ieee-1", 1).AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<ZigbeeDeviceJoinedEvent>>(routableEvent);
    }

    [Fact]
    public void WhenZigbeeDeviceInfoDiscoveredThenReturnsRoutableEventFromZigbeeDeviceInfoDiscovered()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(
            new ZigbeeDeviceInfoDiscoveredEvent("ieee-1", "Acme", "Widget", []).AsHausEvent()
        );

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<ZigbeeDeviceInfoDiscoveredEvent>>(routableEvent);
    }

    [Fact]
    public void WhenZigbeeAttributeReportReceivedThenReturnsRoutableEventFromZigbeeAttributeReportReceived()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(
            new ZigbeeAttributeReportReceivedEvent(1, "ieee-1", 1, 1, 0, 0, null).AsHausEvent()
        );

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<ZigbeeAttributeReportReceivedEvent>>(routableEvent);
    }

    [Fact]
    public void WhenZigbeeCommandSentThenReturnsRoutableEventFromZigbeeCommandSent()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new ZigbeeCommandSentEvent(1, "ieee-1", 1, 1).AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<ZigbeeCommandSentEvent>>(routableEvent);
    }

    [Fact]
    public void WhenZigbeeTransportErrorThenReturnsRoutableEventFromZigbeeTransportError()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(
            new ZigbeeTransportErrorEvent("timeout", "boom", 1, "ieee-1").AsHausEvent()
        );

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<ZigbeeTransportErrorEvent>>(routableEvent);
    }

    [Fact]
    public void WhenZigbeeCommandDroppedThenReturnsRoutableEventFromZigbeeCommandDropped()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(
            new ZigbeeCommandDroppedEvent("ext-1", "no known network address").AsHausEvent()
        );

        var routableEvent = _factory.Create(bytes);

        Assert.IsType<RoutableEvent<ZigbeeCommandDroppedEvent>>(routableEvent);
    }

    [Fact]
    public void WhenBytesDoesNotRepresentAHausEventThenReturnsNull()
    {
        var bytes = HausJsonSerializer.SerializeToBytes("this is data");

        var routableEvent = _factory.Create(bytes);

        Assert.Null(routableEvent);
    }
}
