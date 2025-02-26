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
public class RoomOccupancyTests
{
    private readonly HausWebHostApplicationFactory _factory;
    private readonly IHausApiClient _apiClient;
    private readonly ConcurrentBag<RoomLightingChangedEvent> _roomLightingCommands;

    public RoomOccupancyTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _apiClient = _factory.CreateAuthenticatedClient();
        _roomLightingCommands = [];
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomStaysOnDuringOccupancyTimeout()
    {
        var (room, device) = await SetupRoomWithDevice();
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));
        _roomLightingCommands.Clear();

        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId));
        await Task.Delay(TimeSpan.FromSeconds(3));

        Eventually.Assert(() =>
        {
            _roomLightingCommands
                .Should()
                .NotContain(cmd => cmd.Room.Id == room.Id && cmd.Lighting.State == LightingState.Off);
        });
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomTurnsOffAfterOccupancyTimeout()
    {
        var (room, device) = await SetupRoomWithDevice();
        await _apiClient.UpdateRoomAsync(room.Id, new RoomModel(room.Id, room.Name, 0));

        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId));

        Eventually.Assert(() =>
        {
            _roomLightingCommands.Should().Contain(cmd => cmd.Lighting.State == LightingState.Off);
        });
    }

    [Fact]
    public async Task WhenRoomRemainsVacantThenRoomLightingIsTurnedOff()
    {
        var (room, device) = await SetupRoomWithDevice();
        await _apiClient.UpdateRoomAsync(room.Id, new RoomModel(room.Id, room.Name, 0));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));

        Eventually.Assert(() =>
        {
            _roomLightingCommands.Should().Contain(cmd => cmd.Lighting.State == LightingState.Off);
        });
    }

    private async Task<(RoomModel, DeviceModel)> SetupRoomWithDevice()
    {
        var result = await _factory.AddRoomWithDevice($"{Guid.NewGuid()}", DeviceType.MotionSensor);
        await _factory.SubscribeToRoomLightingChangedCommandsAsync(cmd =>
        {
            if (cmd.Payload != null)
            {
                _roomLightingCommands.Add(cmd.Payload);
            }
        });
        return result;
    }
}
