using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Rooms;

[Collection(HausWebHostCollectionFixture.Name)]
public class RoomOccupancyTests : IAsyncLifetime
{
    private readonly HausWebHostApplicationFactory _factory;
    private readonly IHausApiClient _apiClient;
    private readonly ConcurrentBag<RoomLightingChangedEvent> _roomLightingCommands;
    private RoomModel _room;
    private DeviceModel _device;

    public RoomOccupancyTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _apiClient = _factory.CreateAuthenticatedClient();
        _roomLightingCommands = new ConcurrentBag<RoomLightingChangedEvent>();
    }

    public async Task InitializeAsync()
    {
        var (room, device) = await _factory.AddRoomWithDevice($"{Guid.NewGuid()}", DeviceType.MotionSensor);
        await _factory.SubscribeToRoomLightingChangedCommandsAsync(
            cmd => _roomLightingCommands.Add(cmd.Payload)
        );
        _room = room;
        _device = device;
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomStaysOnDuringOccupancyTimeout()
    {
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));
        _roomLightingCommands.Clear();

        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId));
        await Task.Delay(TimeSpan.FromSeconds(3));

        Eventually.Assert(() =>
        {
            _roomLightingCommands.Should()
                .NotContain(cmd => cmd.Room.Id == _room.Id && cmd.Lighting.State == LightingState.Off);
        });
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomTurnsOffAfterOccupancyTimeout()
    {
        await _apiClient.UpdateRoomAsync(_room.Id, new RoomModel(_room.Id, _room.Name, 0));

        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId));

        Eventually.Assert(() =>
        {
            _roomLightingCommands.Should().Contain(cmd => cmd.Lighting.State == LightingState.Off);
        });
    }

    [Fact]
    public async Task WhenRoomRemainsVacantThenRoomLightingIsTurnedOff()
    {
        await _apiClient.UpdateRoomAsync(_room.Id, new RoomModel(_room.Id, _room.Name, 0));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));

        Eventually.Assert(() =>
        {
            _roomLightingCommands.Should().Contain(cmd => cmd.Lighting.State == LightingState.Off);
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;
}