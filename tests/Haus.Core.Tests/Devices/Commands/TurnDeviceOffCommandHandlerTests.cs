using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands;

public class TurnDeviceOffCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public TurnDeviceOffCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDeviceTurnedOffThenLightingStateIsChangedToOff()
    {
        var device = _context.AddDevice(deviceType: DeviceType.Light, configure: d => d.TurnOn(_hausBus));

        await _hausBus.ExecuteCommandAsync(new TurnDeviceOffCommand(device.Id));

        var lightingCommand = _hausBus.GetPublishedHausCommands<DeviceLightingChangedEvent>().Single();
        lightingCommand.Payload?.Device.Id.Should().Be(device.Id);
        lightingCommand.Payload?.Lighting?.State.Should().Be(LightingState.Off);
    }

    [Fact]
    public async Task WhenDeviceIsMissingThenThrowsNotFoundException()
    {
        var command = new TurnDeviceOffCommand(98234);

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<EntityNotFoundException<DeviceEntity>>();
    }
}
