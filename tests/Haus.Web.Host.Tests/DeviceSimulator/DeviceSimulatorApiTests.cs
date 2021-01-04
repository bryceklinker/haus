using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.DeviceSimulator;
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

            await _client.AddSimulatedDeviceAsync(new CreateSimulatedDeviceModel(DeviceType.Light));

            Eventually.Assert(() =>
            {
                discoveredEvent.DeviceType.Should().Be(DeviceType.Light);
            });
        }

        [Fact]
        public async Task WhenDeviceSimulatorIsResetThenStateIsSetToInitialState()
        {
            await _client.AddSimulatedDeviceAsync(new CreateSimulatedDeviceModel(DeviceType.Light));
            await _client.AddSimulatedDeviceAsync(new CreateSimulatedDeviceModel(DeviceType.Light));
            await _client.AddSimulatedDeviceAsync(new CreateSimulatedDeviceModel(DeviceType.Light));

            DeviceSimulatorState state = null;
            
            var hub = await _factory.CreateHubConnection("device-simulator");
            hub.On<DeviceSimulatorState>("OnState", s => state = s);

            await _client.ResetDeviceSimulatorAsync();
            Eventually.Assert(() =>
            {
                state.Devices.Should().BeEmpty();
            });
        }
    }
}