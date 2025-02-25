using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Events;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.Events;

public class DeviceSimulatorDeviceLightingChangedEventHandlerTests
{
    private readonly IHausBus _hausBus;
    private readonly IDeviceSimulatorStore _simulatorStore;

    public DeviceSimulatorDeviceLightingChangedEventHandlerTests()
    {
        _simulatorStore = new DeviceSimulatorStore();
        _hausBus = HausBusFactory.Create(configureServices: services =>
            services.Replace<IDeviceSimulatorStore>(_simulatorStore)
        );
    }

    [Fact]
    public async Task WhenLightingChangedEventReceivedForSimulatedLightThenSimulatedLightLightingIsUpdated()
    {
        var simulatedLight = SimulatedDeviceEntity.Create(new SimulatedDeviceModel(DeviceType: DeviceType.Light));

        var lighting = new LightingModel(LightingState.On);
        _simulatorStore.PublishNext(s => s.AddSimulatedDevice(simulatedLight));
        await _hausBus.PublishAsync(
            RoutableEvent.FromEvent(
                new DeviceLightingChangedEvent(new DeviceModel(ExternalId: simulatedLight.Id), lighting)
            )
        );

        _simulatorStore
            .Current.Devices.Should()
            .Contain(d => d.Id == simulatedLight.Id && d.Lighting.State == LightingState.On);
    }
}
