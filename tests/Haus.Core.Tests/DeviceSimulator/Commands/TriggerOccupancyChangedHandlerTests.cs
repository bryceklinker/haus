using System;
using System.Threading.Tasks;
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

        var simulatedEvent = Assert.Single(_hausBus.GetPublishedEvents<SimulatedEvent>());
        Assert.True(simulatedEvent.HausEvent is HausEvent<OccupancyChangedModel>);
    }

    [Fact]
    public async Task WhenOccupancyIsTriggeredThenUpdatesSimulatorState()
    {
        await _hausBus.ExecuteCommandAsync(new TriggerOccupancyChangedCommand(_simulatedDeviceId));

        Assert.True(_simulatorStore.GetDeviceById(_simulatedDeviceId)?.IsOccupied);
    }

    [Fact]
    public async Task WhenOccupancyIsTriggeredForAMissingSimulatorThenThrowsException()
    {
        var act = () => _hausBus.ExecuteCommandAsync(new TriggerOccupancyChangedCommand($"{Guid.NewGuid()}"));

        await Assert.ThrowsAsync<SimulatorNotFoundException>(act);
    }
}
