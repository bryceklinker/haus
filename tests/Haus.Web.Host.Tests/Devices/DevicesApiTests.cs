using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
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
            await PublishToMqtt("haus/events", new DeviceDiscoveredModel
            {
                Id = "hello"
            }.AsHausEvent());

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
        public async Task WhenDiscoveryIsStartedThenStartDiscoveryCommandIsPublished()
        {
            var mqttClient = await _factory.GetMqttClient();

            HausCommand<StartDiscoveryModel> hausCommand = null;
            await mqttClient.SubscribeToTopicAsync("haus/commands", msg => HausJsonSerializer.TryDeserialize(msg.Payload, out hausCommand));

            await _hausClient.StartDiscovery();

            await Eventually.Assert(() =>
            {
                Assert.Equal(StartDiscoveryModel.Type, hausCommand.Type);
            });
        }

        private async Task PublishToMqtt(string topic, object payload)
        {
            var mqttClient = await _factory.GetMqttClient();
            await mqttClient.PublishAsync(topic, payload);
        }
    }
}