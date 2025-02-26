using System;
using FluentAssertions;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.State;

public class DeviceSimulatorStateTests
{
    [Fact]
    public void WhenSimulatedDeviceIsAddedThenStateHasSimulatedDevice()
    {
        var entity = SimulatedDeviceEntity.Create(new SimulatedDeviceModel());

        var state = DeviceSimulatorState.Initial.AddSimulatedDevice(entity);

        state.Devices.Should().Contain(entity);
    }

    [Fact]
    public void WhenResetThenReturnsInitialState()
    {
        var entity = SimulatedDeviceEntity.Create(new SimulatedDeviceModel());

        var state = DeviceSimulatorState.Initial.AddSimulatedDevice(entity).Reset();

        state.Should().Be(DeviceSimulatorState.Initial);
    }

    [Fact]
    public void WhenConvertedToModelThenDevicesAreInModel()
    {
        var model = DeviceSimulatorState
            .Initial.AddSimulatedDevice(SimulatedDeviceEntity.Create(new SimulatedDeviceModel()))
            .ToModel();

        model.Devices.Should().HaveCount(1);
    }

    [Fact]
    public void WhenDeviceLightingIsChangedThenReturnsUpdatedDeviceInState()
    {
        var deviceId = $"{Guid.NewGuid()}";
        var state = DeviceSimulatorState
            .Initial.AddSimulatedDevice(
                SimulatedDeviceEntity.Create(new SimulatedDeviceModel(deviceId, DeviceType.Light))
            )
            .ChangeDeviceLighting(deviceId, new LightingModel(LightingState.On));

        state
            .Devices.Should()
            .HaveCount(1)
            .And.OnlyContain(e => e.Id == deviceId && e.Lighting != null && e.Lighting.State == LightingState.On);
    }

    [Fact]
    public void WhenDeviceLightingChangesForDeviceNotInStateThenReturnsUnchangedState()
    {
        var state = DeviceSimulatorState
            .Initial.AddSimulatedDevice(SimulatedDeviceEntity.Create(new SimulatedDeviceModel()))
            .ChangeDeviceLighting($"{Guid.NewGuid()}", new LightingModel());

        state.Devices.Should().HaveCount(1);
    }

    [Fact]
    public void WhenDeviceOccupancyChangedThenReturnsStateWithUpdatedDevice()
    {
        var deviceId = $"{Guid.NewGuid()}";
        var state = DeviceSimulatorState
            .Initial.AddSimulatedDevice(
                SimulatedDeviceEntity.Create(new SimulatedDeviceModel(deviceId, DeviceType.MotionSensor))
            )
            .ChangeOccupancy(deviceId);

        state.Devices.Should().HaveCount(1).And.OnlyContain(e => e.Id == deviceId && e.IsOccupied);
    }

    [Fact]
    public void WhenDeviceOccupancyChangedForMissingDeviceThenReturnsUnchangedState()
    {
        var state = DeviceSimulatorState.Initial.ChangeOccupancy($"{Guid.NewGuid()}");

        state.Should().BeSameAs(DeviceSimulatorState.Initial);
    }
}
