using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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
    private static readonly DateTime BaseClockTime = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const int OccupancyTimeoutInSeconds = 3600;

    // Deliberately far outside the range any other fact in this shared-clock collection ever
    // advances to, so this test's still-on room can never become collaterally eligible for
    // RoomVacancyBackgroundService's sweep once a later fact jumps the (process-wide) FakeClock.
    private const int NeverExpiresTimeoutInSeconds = 1_000_000;

    private readonly HausWebHostApplicationFactory _factory;
    private readonly IHausApiClient _apiClient;
    private readonly ConcurrentBag<RoomLightingChangedEvent> _roomLightingCommands;

    public RoomOccupancyTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _apiClient = _factory.CreateAuthenticatedClient();
        _roomLightingCommands = [];
        _factory.SetClockTime(BaseClockTime);
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomStaysOnDuringOccupancyTimeout()
    {
        var (room, device) = await SetupRoomWithDevice();
        await _apiClient.UpdateRoomAsync(room.Id, new RoomModel(room.Id, room.Name, NeverExpiresTimeoutInSeconds));

        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId, true));
        Eventually.Assert(() =>
        {
            Assert.Contains(
                _roomLightingCommands,
                cmd => cmd.Room.Id == room.Id && cmd.Lighting.State == LightingState.On
            );
        });
        _roomLightingCommands.Clear();

        _factory.SetClockTime(BaseClockTime.AddSeconds(NeverExpiresTimeoutInSeconds - 1));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId));
        await Task.Delay(TimeSpan.FromSeconds(1));

        Eventually.Assert(() =>
        {
            Assert.DoesNotContain(
                _roomLightingCommands,
                cmd => cmd.Room.Id == room.Id && cmd.Lighting.State == LightingState.Off
            );
        });
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomTurnsOffAfterOccupancyTimeout()
    {
        var (room, device) = await SetupRoomWithDevice();
        await _apiClient.UpdateRoomAsync(room.Id, new RoomModel(room.Id, room.Name, OccupancyTimeoutInSeconds));

        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId, true));
        Eventually.Assert(() =>
        {
            Assert.Contains(
                _roomLightingCommands,
                cmd => cmd.Room.Id == room.Id && cmd.Lighting.State == LightingState.On
            );
        });

        _factory.SetClockTime(BaseClockTime.AddSeconds(OccupancyTimeoutInSeconds + 1));

        // Once the clock passes the timeout, RoomVacancyBackgroundService's own poll also
        // becomes eligible to turn this room off; give it a full cycle to land its write
        // before publishing our own vacant event, so the two writers don't race the same row.
        await Task.Delay(TimeSpan.FromSeconds(1));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId));

        Eventually.Assert(() =>
        {
            Assert.Contains(
                _roomLightingCommands,
                cmd => cmd.Room.Id == room.Id && cmd.Lighting.State == LightingState.Off
            );
        });
    }

    [Fact]
    public async Task WhenRoomRemainsVacantThenRoomLightingIsTurnedOff()
    {
        var (room, device) = await SetupRoomWithDevice();
        await _apiClient.UpdateRoomAsync(room.Id, new RoomModel(room.Id, room.Name, OccupancyTimeoutInSeconds));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(device.ExternalId, true));
        Eventually.Assert(() =>
        {
            Assert.Contains(
                _roomLightingCommands,
                cmd => cmd.Room.Id == room.Id && cmd.Lighting.State == LightingState.On
            );
        });

        _factory.SetClockTime(BaseClockTime.AddSeconds(OccupancyTimeoutInSeconds + 1));

        Eventually.Assert(() =>
        {
            Assert.Contains(
                _roomLightingCommands,
                cmd => cmd.Room.Id == room.Id && cmd.Lighting.State == LightingState.Off
            );
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
