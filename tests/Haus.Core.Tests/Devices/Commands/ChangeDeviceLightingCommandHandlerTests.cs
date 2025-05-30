using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands;

public class ChangeDeviceLightingCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public ChangeDeviceLightingCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDeviceLightingChangedThenDeviceLightingIsSavedToDatabase()
    {
        var device = _context.AddDevice(deviceType: DeviceType.Light);

        var lighting = new LightingModel(Level: new LevelLightingModel(54));
        await _hausBus.ExecuteCommandAsync(new ChangeDeviceLightingCommand(device.Id, lighting));

        var updated = await _context.FindByIdAsync<DeviceEntity>(device.Id);
        updated?.Lighting?.Level.Should().BeEquivalentTo(new LevelLightingEntity(54));
    }

    [Fact]
    public async Task WhenDeviceIsMissingThenThrowsEntityNotFoundException()
    {
        var command = new ChangeDeviceLightingCommand(4213, new LightingModel());

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<EntityNotFoundException<DeviceEntity>>();
    }

    [Fact]
    public async Task WhenDeviceIsNotALightThenThrowsInvalidOperationException()
    {
        var device = _context.AddDevice();
        var lighting = new LightingModel();
        var command = new ChangeDeviceLightingCommand(device.Id, lighting);

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task WhenDeviceLightingIsChangedThenDeviceLightingChangedIsPublished()
    {
        var device = _context.AddDevice(deviceType: DeviceType.Light);

        var lighting = new LightingModel(Level: new LevelLightingModel(65));
        await _hausBus.ExecuteCommandAsync(new ChangeDeviceLightingCommand(device.Id, lighting));

        var hausCommand = _hausBus.GetPublishedHausCommands<DeviceLightingChangedEvent>().Single();
        hausCommand.Payload?.Device.Id.Should().Be(device.Id);
        hausCommand.Payload?.Lighting?.Level.Should().BeEquivalentTo(new LevelLightingModel(65));
    }
}
