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
            var entity = SimulatedDeviceEntity.Create(new CreateSimulatedDeviceModel());
            
            var state = DeviceSimulatorState.Initial.AddSimulatedDevice(entity);

            state.Devices.Should().Contain(entity);
        }
    }
}