using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
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
    private RoomModel _room;
    private DeviceModel _device;

    public RoomOccupancyTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _apiClient = _factory.CreateAuthenticatedClient();
    }

    public async Task InitializeAsync()
    {
        var (room, device) = await _factory.AddRoomWithDevice($"{Guid.NewGuid()}", DeviceType.MotionSensor);
        _room = room;
        _device = device;
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomStaysOnDuringOccupancyTimeout()
    {
        var commands = new ConcurrentBag<HausCommand<RoomLightingChangedEvent>>();
        await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(
            cmd => commands.Add(cmd) 
        );
        
        await _apiClient.UpdateRoomAsync(_room.Id, new RoomModel(_room.Id, _room.Name, 500));
        
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId));
        await Task.Delay(TimeSpan.FromSeconds(3));

        Eventually.Assert(() =>
        {
            commands.Should().Contain(cmd => cmd.Type == RoomLightingChangedEvent.Type)
                .And.NotContain(cmd => cmd.Payload.Lighting.State == LightingState.Off);
        });
    }

    [Fact]
    public async Task WhenRoomHasMotionSensorThenRoomTurnsOffAfterOccupancyTimeout()
    {
        var commands = new ConcurrentBag<HausCommand<RoomLightingChangedEvent>>();
        await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(
            cmd => commands.Add(cmd) 
        );
        
        await _apiClient.UpdateRoomAsync(_room.Id, new RoomModel(_room.Id, _room.Name, 2));
        
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId, true));
        await Task.Delay(TimeSpan.FromSeconds(3));
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(_device.ExternalId));

        Eventually.Assert(() =>
        {
            commands.Should().Contain(evt => evt.Type == RoomLightingChangedEvent.Type)
                .And.Contain(evt => evt.Payload.Lighting.State == LightingState.Off);
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;
}