using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.DeviceSimulator.Commands;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.Events;
using Haus.Core.DeviceSimulator.Exceptions;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.Commands;

public class TriggerOccupancyChangedHandlerTests
{
    private readonly string _simulatedDeviceId;
    private readonly IDeviceSimulatorStore _simulatorStore;
    private readonly CapturingHausBus _hausBus;

    public TriggerOccupancyChangedHandlerTests()
    {
        _simulatedDeviceId = $"{Guid.NewGuid()}";
        _simulatorStore = new DeviceSimulatorStore();
        _simulatorStore.PublishNext(s =>
            s.AddSimulatedDevice(new SimulatedDeviceEntity(_simulatedDeviceId, DeviceType.MotionSensor))
        );

        _hausBus = HausBusFactory.CreateCapturingBus(_simulatorStore);
    }

    [Fact]
    public async Task WhenOccupancyIsTriggeredThenPublishesSimulatedOccupancyChangedEvent()
    {
        await _hausBus.ExecuteCommandAsync(new TriggerOccupancyChangedCommand(_simulatedDeviceId));

        _hausBus
            .GetPublishedEvents<SimulatedEvent>()
            .Should()
            .HaveCount(1)
            .And.Contain(e => e.HausEvent is HausEvent<OccupancyChangedModel>);
    }

    [Fact]
    public async Task WhenOccupancyIsTriggeredThenUpdatesSimulatorState()
    {
        await _hausBus.ExecuteCommandAsync(new TriggerOccupancyChangedCommand(_simulatedDeviceId));

        _simulatorStore.GetDeviceById(_simulatedDeviceId).IsOccupied.Should().BeTrue();
    }

    [Fact]
    public async Task WhenOccupancyIsTriggeredForAMissingSimulatorThenThrowsException()
    {
        var act = () => _hausBus.ExecuteCommandAsync(new TriggerOccupancyChangedCommand($"{Guid.NewGuid()}"));

        await act.Should().ThrowAsync<SimulatorNotFoundException>();
    }
}
