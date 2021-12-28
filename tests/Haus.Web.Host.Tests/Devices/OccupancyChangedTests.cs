using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;
using Xunit.Sdk;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class OccupancyChangedTests
{
    private readonly HausWebHostApplicationFactory _factory;
    private readonly IHausApiClient _apiClient;

    public OccupancyChangedTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _apiClient = _factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task WhenOccupancyChangedForDeviceInRoomThenRoomLightingChangedPublished()
    {
        var (room, sensor) = await _factory.AddRoomWithDevice("home", DeviceType.MotionSensor);

        var lightingCommands = new ConcurrentBag<HausCommand<RoomLightingChangedEvent>>();
        await _factory.SubscribeToRoomLightingChangedCommandsAsync(lightingCommands.Add);
        await _factory.PublishHausEventAsync(new OccupancyChangedModel(sensor.ExternalId, true));

        Eventually.Assert(() =>
        {
            lightingCommands.Should()
                .Contain(cmd => cmd.Payload.Room.Id == room.Id && cmd.Payload.Lighting.State == LightingState.On);
        });
    }
}