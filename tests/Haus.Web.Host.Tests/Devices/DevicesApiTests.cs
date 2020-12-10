using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DevicesApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;
        private readonly IHausApiClient _hausClient;

        public DevicesApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _hausClient = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenADeviceIsUpdatedThenUpdatedDeviceIsAvailableFromTheApi()
        {
            await _factory.PublishHausEventAsync(new DeviceDiscoveredModel
            {
                Id = "hello"
            });

            await Eventually.AssertAsync(async () =>
            {
                var result = await _hausClient.GetDevicesAsync("hello");
                var device = result.Items[0];
                await _hausClient.UpdateDeviceAsync(device.Id, new DeviceModel
                {
                    Name = "some-name"
                });

                var updated = await _hausClient.GetDeviceAsync(device.Id);
                Assert.Equal("some-name", updated.Name);
            });
        }

        [Fact]
        public async Task WhenLightingOfDeviceIsChangedThenDeviceLightingChangedEventIsPublished()
        {
            HausCommand<DeviceLightingChangedEvent> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(msg => hausCommand = msg);
            
            var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
            await _hausClient.ChangeDeviceLighting(device.Id, new LightingModel {Brightness = 5});
            
            Eventually.Assert(() =>
            {
                Assert.Equal(DeviceLightingChangedEvent.Type, hausCommand.Type);
            });
        }

        [Fact]
        public async Task WhenDeviceIsTurnedOffThenPublishesChangeLightingWithOffState()
        {
            HausCommand<DeviceLightingChangedEvent> published = null;
            await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(msg => published = msg);

            var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
            await _hausClient.TurnLightOff(device.Id);
            
            Eventually.Assert(() =>
            {
                Assert.Equal(LightingState.Off, published.Payload.Lighting.State);
            });
        }
        
        [Fact]
        public async Task WhenDeviceIsTurnedOnThenPublishesChangeLightingWithOnState()
        {
            HausCommand<DeviceLightingChangedEvent> published = null;
            await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(msg => published = msg);

            var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
            await _hausClient.TurnLightOn(device.Id);
            
            Eventually.Assert(() =>
            {
                Assert.Equal(LightingState.On, published.Payload.Lighting.State);
            });
        }
        
        [Fact]
        public async Task WhenDiscoveryIsStartedThenStartDiscoveryCommandIsPublished()
        {
            var mqttClient = await _factory.GetMqttClient();

            HausCommand<StartDiscoveryModel> hausCommand = null;
            await mqttClient.SubscribeToTopicAsync("haus/commands",
                msg => HausJsonSerializer.TryDeserialize(msg.Payload, out hausCommand));

            await _hausClient.StartDiscovery();

            Eventually.Assert(() => { Assert.Equal(StartDiscoveryModel.Type, hausCommand.Type); });
        }

        [Fact]
        public async Task WhenDiscoveryStoppedThenStopDiscoveryCommandIsPublished()
        {
            var mqttClient = await _factory.GetMqttClient();

            HausCommand<StopDiscoveryModel> hausCommand = null;
            await mqttClient.SubscribeToTopicAsync("haus/commands",
                msg => HausJsonSerializer.TryDeserialize(msg.Payload, out hausCommand));

            await _hausClient.StopDiscovery();

            Eventually.Assert(() => { Assert.Equal(StopDiscoveryModel.Type, hausCommand.Type); });
        }
    }
}