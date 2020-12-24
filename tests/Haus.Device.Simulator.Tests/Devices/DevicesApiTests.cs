using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Device.Simulator.Test.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Device.Simulator.Test.Devices
{
    [Collection(DeviceSimulatorCollectionFixture.Name)]
    public class LightsApiTests
    {
        private readonly DeviceSimulatorWebApplication _factory;
        private readonly HttpClient _client;
        private readonly List<DeviceDiscoveredModel> _devicesDiscovered;

        public LightsApiTests(DeviceSimulatorWebApplication factory)
        {
            _factory = factory;
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