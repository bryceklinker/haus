using FluentAssertions;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.DeviceSimulator;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.State
{
    public class DeviceSimulatorStoreTests
    {
        [Fact]
        public void WhenCreatedThenInitialState()
        {
            var store = new DeviceSimulatorStore();

            store.Current.Should().Be(DeviceSimulatorState.Initial);
        }

        [Fact]
        public void WhenDeviceIsAddedThenStateIsUpdatedToHaveDevice()
        {
            var device = SimulatedDeviceEntity.Create(new SimulatedDeviceModel());

            var store = new DeviceSimulatorStore();
            store.Publish(store.Current.AddSimulatedDevice(device));

            store.Current.Devices.Should().Contain(device);
        }
    }
}