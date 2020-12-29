using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Models.DeviceSimulator;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.DeviceSimulator
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DeviceSimulatorRealtimeApiTests
    {
        private readonly IHausApiClient _client;
        private readonly HausWebHostApplicationFactory _factory;

        public DeviceSimulatorRealtimeApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenSimulatedDeviceIsAddedThenStateContainsSimulatedDevice()
        {
            await _client.ResetDeviceSimulatorAsync();
            
            var hub = await _factory.CreateHubConnection("device-simulator");
            
            DeviceSimulatorState state = null;
            hub.On<DeviceSimulatorState>("OnState", s => state = s);

            await _client.AddSimulatedDeviceAsync(new CreateSimulatedDeviceModel());
            
            Eventually.Assert(() =>
            {
                state.Devices.Should().HaveCount(1);
            });
        }
    }
}