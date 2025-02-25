using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.DeviceSimulator.Commands;
using Haus.Core.DeviceSimulator.State;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.Commands;

public class ResetDeviceSimulatorCommandHandlerTests
{
    private readonly IDeviceSimulatorStore _store;
    private readonly IHausBus _hausBus;

    public ResetDeviceSimulatorCommandHandlerTests()
    {
        _store = new DeviceSimulatorStore();
        _hausBus = HausBusFactory.Create(configureServices: services =>
            services.Replace<IDeviceSimulatorStore>(_store)
        );
    }

    [Fact]
    public async Task WhenDeviceSimulatorIsResetThenStateIsSetToInitialState()
    {
        IDeviceSimulatorState state = null;
        _store.Subscribe(s => state = s);

        await _hausBus.ExecuteCommandAsync(new ResetDeviceSimulatorCommand());

        state.Devices.Should().BeEmpty();
    }
}
