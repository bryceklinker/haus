using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Device.Simulator.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Device.Simulator.Tests.Devices
{
    public class DevicesApiTests 
    {
        private readonly DeviceSimulatorWebApplication _factory;
        private readonly HttpClient _client;
        private readonly List<DeviceDiscoveredModel> _devicesDiscovered;

        public DevicesApiTests()
        {
            _factory = new DeviceSimulatorWebApplication();
            _client = _factory.CreateClient();
            _devicesDiscovered = new List<DeviceDiscoveredModel>();
        }

        [Fact]
        public async Task WhenLightAddedThenAnnouncesLightDiscovered()
        {
            await SubscribeToDeviceDiscoveredEvents();

            await _client.PostAsJsonAsync("/api/devices", new DeviceModel
            {
                DeviceType = DeviceType.Light
            });

            Eventually.Assert(() => { AssertDeviceWasAnnounced(DeviceType.Light); });
        }

        [Fact]
        public async Task WhenMultiSensorIsAddedThenAnnouncesMultiSensorDiscovered()
        {
            await SubscribeToDeviceDiscoveredEvents();

            await _client.PostAsync("/api/devices/clear", new StringContent(string.Empty));
            await _client.PostAsJsonAsync("/api/devices", new DeviceModel
            {
                DeviceType = DeviceType.MotionSensor | DeviceType.LightSensor | DeviceType.TemperatureSensor
            });

            Eventually.Assert(() =>
            {
                AssertDeviceWasAnnounced(DeviceType.MotionSensor | DeviceType.LightSensor |
                                         DeviceType.TemperatureSensor);
            });
        }

        [Fact]
        public async Task WhenGettingDeviceTypesThenReturnsAllAvailableDeviceTypes()
        {
            var deviceTypes = await _client.GetFromJsonAsync<string[]>("/api/deviceTypes");

            Assert.Contains(Enum.GetNames(typeof(DeviceType)), deviceTypeName => deviceTypes.Contains(deviceTypeName));
        }

        [Fact]
        public async Task WhenDevicesAreClearedThenDevicesIsEmpty()
        {
            await _client.PostAsJsonAsync("/api/devices", new DeviceModel {DeviceType = DeviceType.Light});
            await _client.PostAsJsonAsync("/api/devices", new DeviceModel {DeviceType = DeviceType.Light});
            await _client.PostAsJsonAsync("/api/devices", new DeviceModel {DeviceType = DeviceType.Light});
            await _client.PostAsync("/api/devices/clear", new StringContent(string.Empty));

            var devices = await _client.GetFromJsonAsync<ListResult<DeviceModel>>("/api/devices");
            Assert.Empty(devices.Items);
            Assert.Equal(0, devices.Count);
        }
        
        private async Task SubscribeToDeviceDiscoveredEvents()
        {
            var mqttClient = await _factory.GetMqttClientAsync();
            await mqttClient.SubscribeToHausEventsAsync<DeviceDiscoveredModel>(e => _devicesDiscovered.Add(e.Payload));
        }

        private void AssertDeviceWasAnnounced(DeviceType deviceType)
        {
            Assert.Contains(_devicesDiscovered, model => !string.IsNullOrEmpty(model.Id)
                                                         && model.DeviceType == deviceType
                                                         && model.GetMetadataValue("SIMULATED") == "True"
            );
        }
    }
}