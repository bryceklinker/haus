using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands;

public class UpdateDeviceCommandTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public UpdateDeviceCommandTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenUpdateDeviceCommandExecutedThenDeviceIsUpdatedInDatabase()
    {
        var original = _context.AddDevice();
        var model = new DeviceModel(original.Id, Name: "hi-bob");

        await _hausBus.ExecuteCommandAsync(new UpdateDeviceCommand(model));

        var updated = await _context.FindAsync<DeviceEntity>(original.Id);
        updated?.Name.Should().Be("hi-bob");
    }

    [Fact]
    public async Task WhenDeviceUpdatedThenDeviceUpdatedEventPublished()
    {
        var original = _context.AddDevice();
        var model = new DeviceModel(original.Id, Name: "hi-bob");

        await _hausBus.ExecuteCommandAsync(new UpdateDeviceCommand(model));

        _hausBus.GetPublishedRoutableEvents<DeviceUpdatedEvent>().Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenUpdateDeviceCommandExecutedForMissingDeviceThenThrowsException()
    {
        var command = new UpdateDeviceCommand(new DeviceModel(Name: "hello"));

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<EntityNotFoundException<DeviceEntity>>();
    }

    [Fact]
    public async Task WhenUpdateDeviceCommandIsInvalidThenThrowsException()
    {
        var device = _context.AddDevice();
        var command = new UpdateDeviceCommand(new DeviceModel(device.Id, Name: null));

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<HausValidationException>();
    }
}
