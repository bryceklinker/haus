using System;
using FluentAssertions;
using Haus.Core.Common.Events;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Xunit;

namespace Haus.Core.Tests.Common.Events;

public class RoutableHausEventFactoryTest
{
    private readonly RoutableEventFactory _factory;

    public RoutableHausEventFactoryTest()
    {
        _factory = new RoutableEventFactory();
    }

    [Fact]
    public void WhenDeviceDiscoveredEventThenReturnsRoutableEventFromDeviceDiscovered()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new DeviceDiscoveredEvent($"{Guid.NewGuid()}").AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        routableEvent.Should().BeOfType<RoutableEvent<DeviceDiscoveredEvent>>();
    }

    [Fact]
    public void WhenMultiSensorChangedThenReturnsRoutableEventFromMultiSensorChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new MultiSensorChanged($"{Guid.NewGuid()}").AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        routableEvent.Should().BeOfType<RoutableEvent<MultiSensorChanged>>();
    }

    [Fact]
    public void WhenMotionSensorChangedThenReturnsRoutableEventFromMotionSensorChanged()
    {
        var bytes = HausJsonSerializer.SerializeToBytes(new OccupancyChangedModel($"{Guid.NewGuid()}").AsHausEvent());

        var routableEvent = _factory.Create(bytes);

        routableEvent.Should().BeOfType<RoutableEvent<OccupancyChangedModel>>();
    }

    [Fact]
    public void WhenBytesDoesNotRepresentAHausEventThenReturnsNull()
    {
        var bytes = HausJsonSerializer.SerializeToBytes("this is data");

        var routableEvent = _factory.Create(bytes);

        routableEvent.Should().BeNull();
    }
}