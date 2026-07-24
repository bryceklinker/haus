using System.Linq;
using System.Threading.Tasks;
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

public class TurnDeviceOnCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public TurnDeviceOnCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDeviceIsTurnedOnThenPublishedChangeLightingWithStateOn()
    {
        var device = _context.AddDevice(deviceType: DeviceType.Light, configure: d => d.TurnOff(_hausBus));

        await _hausBus.ExecuteCommandAsync(new TurnDeviceOnCommand(device.Id));

        var publishedCommand = _hausBus.GetPublishedHausCommands<DeviceLightingChangedEvent>().Single();
        Assert.Equal(device.Id, publishedCommand.Payload?.Device.Id);
        Assert.Equal(LightingState.On, publishedCommand.Payload?.Lighting?.State);
    }

    [Fact]
    public async Task WhenDeviceIsMissingThenThrowsNotFound()
    {
        var command = new TurnDeviceOnCommand(324);

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await Assert.ThrowsAsync<EntityNotFoundException<DeviceEntity>>(act);
    }
}
