using FluentAssertions;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.DeviceSimulator;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.State
{
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

            var state = DeviceSimulatorState.Initial
                .AddSimulatedDevice(entity)
                .Reset();

            state.Should().Be(DeviceSimulatorState.Initial);
        }

        [Fact]
        public void WhenConvertedToModelThenDevicesAreInModel()
        {
            var model = DeviceSimulatorState.Initial
                .AddSimulatedDevice(SimulatedDeviceEntity.Create(new SimulatedDeviceModel()))
                .ToModel();

            model.Devices.Should().HaveCount(1);
        }
    }
}