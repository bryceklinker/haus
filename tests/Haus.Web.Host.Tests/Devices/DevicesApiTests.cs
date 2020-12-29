using System.Threading.Tasks;
using Haus.Api.Client;
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
            await _hausClient.ChangeDeviceLightingAsync(device.Id, new LightingModel {Brightness = 5});
            
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
            await _hausClient.TurnLightOffAsync(device.Id);
            
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
            await _hausClient.TurnLightOnAsync(device.Id);
            
            Eventually.Assert(() =>
            {
                Assert.Equal(LightingState.On, published.Payload.Lighting.State);
            });
        }
        
        [Fact]
        public async Task WhenDiscoveryIsStartedThenStartDiscoveryCommandIsPublished()
        {
            HausCommand<StartDiscoveryModel> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<StartDiscoveryModel>(cmd => hausCommand = cmd);

            await _hausClient.StartDiscoveryAsync();

            Eventually.Assert(() => { Assert.Equal(StartDiscoveryModel.Type, hausCommand.Type); });
        }

        [Fact]
        public async Task WhenDiscoveryStoppedThenStopDiscoveryCommandIsPublished()
        {
            HausCommand<StopDiscoveryModel> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<StopDiscoveryModel>(cmd => hausCommand = cmd);

            await _hausClient.StopDiscoveryAsync();

            Eventually.Assert(() => { Assert.Equal(StopDiscoveryModel.Type, hausCommand.Type); });
        }
    }
}