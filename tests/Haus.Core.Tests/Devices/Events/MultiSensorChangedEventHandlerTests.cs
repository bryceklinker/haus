using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Events;
using Haus.Core.Models.Devices.Sensors;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Events;

public class MultiSensorChangedEventHandlerTests
{
    private readonly CapturingHausBus _hausBus;

    public MultiSensorChangedEventHandlerTests()
    {
        _hausBus = HausBusFactory.CreateCapturingBus();
    }

    [Fact]
    public async Task WhenMultiSensorChangedHasOccupancyThenPublishesOccupancyChangedEvent()
    {
        var deviceId = $"{Guid.NewGuid()}";
        var change = new MultiSensorChanged(deviceId, new OccupancyChangedModel(deviceId));

        await _hausBus.PublishAsync(RoutableEvent.FromEvent(change));

        _hausBus.GetPublishedEvents<RoutableEvent<OccupancyChangedModel>>()
            .Should().HaveCount(1);
    }
}