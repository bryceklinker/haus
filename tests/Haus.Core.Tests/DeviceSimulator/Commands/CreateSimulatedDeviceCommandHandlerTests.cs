using System.Threading.Tasks;
using Haus.Core.DeviceSimulator.Commands;
using Haus.Core.DeviceSimulator.Events;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.DeviceSimulator;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.Commands;

public class CreateSimulatedDeviceCommandHandlerTests
{
    private readonly CapturingHausBus _hausBus;
    private readonly IDeviceSimulatorStore _store;

    public CreateSimulatedDeviceCommandHandlerTests()
    {
        _store = new DeviceSimulatorStore();
        _hausBus = HausBusFactory.CreateCapturingBus(configureServices: services =>
            services.Replace<IDeviceSimulatorStore>(_store)
        );
    }

    [Fact]
    public async Task WhenSimulatedDeviceCreatedThenDeviceIsAddedToState()
    {
        var command = new CreateSimulatedDeviceCommand(new SimulatedDeviceModel());
        await _hausBus.ExecuteCommandAsync(command);

        Assert.Single(_store.Current.Devices);
    }

    [Fact]
    public async Task WhenSimulatedDeviceCreatedThenSimulatedDeviceDiscoveredEventIsPublished()
    {
        var command = new CreateSimulatedDeviceCommand(new SimulatedDeviceModel());
        await _hausBus.ExecuteCommandAsync(command);

        Assert.Single(
            _hausBus.GetPublishedEvents<SimulatedEvent>(),
            e => e.HausEvent.Type == DeviceDiscoveredEvent.Type
        );
    }
}
