using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.DeviceSimulator
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DeviceSimulatorApiTests
    {
        private readonly IHausApiClient _client;
        private readonly HausWebHostApplicationFactory _factory;

        public DeviceSimulatorApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenSimulatedDeviceIsAddedThenPublishesDeviceDiscoveredEvent()
        {
            DeviceDiscoveredEvent discoveredEvent = null;
            await _factory.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(evt => discoveredEvent = evt.Payload);

            await _client.AddSimulatedDeviceAsync(new SimulatedDeviceModel(DeviceType: DeviceType.Light));

            Eventually.Assert(() =>
            {
                discoveredEvent.DeviceType.Should().Be(DeviceType.Light);
            });
        }

        [Fact]
        public async Task WhenDeviceSimulatorIsResetThenStateIsSetToInitialState()
        {
            await _client.AddSimulatedDeviceAsync(new SimulatedDeviceModel(DeviceType: DeviceType.Light));
            await _client.AddSimulatedDeviceAsync(new SimulatedDeviceModel(DeviceType: DeviceType.Light));
            await _client.AddSimulatedDeviceAsync(new SimulatedDeviceModel(DeviceType: DeviceType.Light));

            DeviceSimulatorStateModel state = null;
            
            var hub = await _factory.CreateHubConnection("device-simulator");
            hub.On<DeviceSimulatorStateModel>("OnState", s => state = s);

            await _client.ResetDeviceSimulatorAsync();
            Eventually.Assert(() =>
            {
                state.Devices.Should().BeEmpty();
            });
        }

        [Fact]
        public async Task WhenTriggeringOccupancyChangeForDeviceInRoomThenRoomIsTurnedOn()
        {
            var simulator = new SimulatedDeviceModel($"{Guid.NewGuid()}", DeviceType.MotionSensor);
            await _client.AddSimulatedDeviceAsync(simulator);
            var device = await _factory.WaitForDeviceToBeDiscovered(simulator.DeviceType, simulator.Id);
            var room = await (await _client.CreateRoomAsync(new RoomModel(Name: $"{Guid.NewGuid()}"))).Content.ReadFromJsonAsync<RoomModel>();
            await _client.AddDevicesToRoomAsync(room.Id, device.Id);
            
            await _client.TriggerOccupancyChange(simulator.Id);

            await Eventually.AssertAsync(async () =>
            {
                var updatedRoom = await _client.GetRoomAsync(room.Id);
                updatedRoom.Lighting.State.Should().Be(LightingState.On);
            });
        }
    }
}