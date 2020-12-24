using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Device.Simulator.Devices.Services;
using Haus.Device.Simulator.Tests.Support;
using Haus.Testing.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Device.Simulator.Tests.Devices
{
    public class DevicesRealTimeApiTests
    {
        private readonly HttpClient _httpClient;
        private readonly DeviceSimulatorWebApplication _factory;

        public DevicesRealTimeApiTests()
        {
            _factory = new DeviceSimulatorWebApplication();
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task WhenConnectedThenReceivesCurrentDevices()
        {
            await _httpClient.PostAsync("/api/devices/clear", new ByteArrayContent(Array.Empty<byte>()));
            var hub = await _factory.CreateHubConnection("devices");

            DevicesState state = null;
            hub.On<DevicesState>("OnStateChange", e => state = e);
            
            await _httpClient.PostAsJsonAsync("/api/devices", new DeviceModel {DeviceType = DeviceType.Light});
            await _httpClient.PostAsJsonAsync("/api/devices", new DeviceModel {DeviceType = DeviceType.Light});
            await _httpClient.PostAsJsonAsync("/api/devices", new DeviceModel {DeviceType = DeviceType.Light});
            Eventually.Assert(() =>
            {
                Assert.Equal(3, state.Devices.Length);
                Assert.Equal(3, state.DevicesById.Count);
            }); 
        }
    }
}