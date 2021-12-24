using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Rooms;

[Collection(HausWebHostCollectionFixture.Name)]
public class RoomsApiTests
{
    private readonly HausWebHostApplicationFactory _factory;
    private readonly IHausApiClient _apiClient;

    public RoomsApiTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _apiClient = _factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task WhenRoomCreatedThenRoomIsAvailableFromGetApi()
    {
        await CreateRoomAsync("Johnny");

        var result = await _apiClient.GetRoomsAsync();

        result.Items.Should().Contain(r => r.Name == "Johnny");
    }

    [Fact]
    public async Task WhenRoomCreatedThenReturnsLocationOfRoom()
    {
        var response = await _apiClient.CreateRoomAsync(new RoomModel(Name: "something"));

        var result = await response.Content.ReadFromJsonAsync<RoomModel>();

        response.Headers.Location.Should().Be($"{_apiClient.ApiBaseUrl}/rooms/{result.Id}");
    }

    [Fact]
    public async Task WhenRoomUpdatedThenGettingRoomReturnsUpdatedRoom()
    {
        var room = await CreateRoomAsync("old");

        var updateResponse = await _apiClient.UpdateRoomAsync(room.Id, new RoomModel(Name: "new hotness"));
        updateResponse.EnsureSuccessStatusCode();

        var updated = await _apiClient.GetRoomAsync(room.Id);

        updated.Name.Should().Be("new hotness");
    }

    [Fact]
    public async Task WhenRoomCreatedWithDuplicateNameThenReturnsBadRequest()
    {
        await CreateRoomAsync("create-duplicate");
            
        var response = await _apiClient.CreateRoomAsync(new RoomModel(Name: "create-duplicate"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
        
    [Fact]
    public async Task WhenRoomUpdatedWithDuplicateNameThenReturnsBadRequest()
    {
        await CreateRoomAsync("duplicate");
        var room = await CreateRoomAsync("update-to-be-duplicate");

        var response = await _apiClient.UpdateRoomAsync(room.Id, new RoomModel(Name: "duplicate"));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
        
    [Fact]
    public async Task WhenDeviceAddedToRoomThenRoomHasDevice()
    {
        var room = await CreateRoomAsync("room with devices");
        var device = await _factory.WaitForDeviceToBeDiscovered();
        await _apiClient.AddDevicesToRoomAsync(room.Id, device.Id);

        var result = await _apiClient.GetDevicesInRoomAsync(room.Id);
        result.Count.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenRoomLightingIsSetThenRoomLightingEventPublishedToMqtt()
    {
        HausCommand<RoomLightingChangedEvent> hausCommand = null;
        await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(msg => hausCommand = msg);
            
        var room = await CreateRoomAsync("room");
        await _apiClient.ChangeRoomLightingAsync(room.Id, new LightingModel(LightingState.On));
            
        Eventually.Assert(() =>
        {
            hausCommand.Type.Should().Be(RoomLightingChangedEvent.Type);
        });
    }

    [Fact]
    public async Task WhenRoomIsTurnedOffThenRoomLightingEventPublishedWithStateOff()
    {
        HausCommand<RoomLightingChangedEvent> hausCommand = null;
        await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(msg => hausCommand = msg);
            
        var room = await CreateRoomAsync("turn-off");
        await _apiClient.TurnRoomOffAsync(room.Id);
            
        Eventually.Assert(() =>
        {
            hausCommand.Type.Should().Be(RoomLightingChangedEvent.Type);
            hausCommand.Payload.Lighting.State.Should().Be(LightingState.Off);
        });
    }
        
    [Fact]
    public async Task WhenRoomIsTurnedOnThenRoomLightingEventPublishedWithStateOn()
    {
        HausCommand<RoomLightingChangedEvent> hausCommand = null;
        await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(msg => hausCommand = msg);
            
        var room = await CreateRoomAsync("turn-on");
        await _apiClient.TurnRoomOnAsync(room.Id);
            
        Eventually.Assert(() =>
        {
            hausCommand.Type.Should().Be(RoomLightingChangedEvent.Type);
            hausCommand.Payload.Lighting.State.Should().Be(LightingState.On);
        });
    }

    [Fact]
    public async Task WhenUnauthenticatedThenRoomsRequestsAreRejected()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var response = await client.CreateRoomAsync(new RoomModel(Name: "something"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<RoomModel> CreateRoomAsync(string name)
    {
        var createResponse = await _apiClient.CreateRoomAsync(new RoomModel(Name: name));
        return await createResponse.Content.ReadFromJsonAsync<RoomModel>();
    }
}