using System;
using System.Reactive.Linq;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.DeviceSimulator;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.State;

public class DeviceSimulatorStoreTests
{
    [Fact]
    public void WhenCreatedThenInitialState()
    {
        var store = new DeviceSimulatorStore();

        Assert.Equal(DeviceSimulatorState.Initial, store.Current);
    }

    [Fact]
    public void WhenDeviceIsAddedThenStateIsUpdatedToHaveDevice()
    {
        var device = SimulatedDeviceEntity.Create(new SimulatedDeviceModel());

        var store = new DeviceSimulatorStore();
        store.Publish(store.Current.AddSimulatedDevice(device));

        Assert.Contains(device, store.Current.Devices);
    }

    [Fact]
    public void WhenPublishNextIsUsedAndNewStateIsUnchangedThenNoUpdatesAreSent()
    {
        var publishCount = 0;

        var store = new DeviceSimulatorStore();
        store.Skip(1).Subscribe(s => publishCount++);

        store.PublishNext(s => s);

        Assert.Equal(0, publishCount);
    }
}
