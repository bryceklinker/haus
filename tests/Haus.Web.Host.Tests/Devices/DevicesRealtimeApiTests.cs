using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Devices
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DevicesRealtimeApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;

        public DevicesRealtimeApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenLightIsAddedToRoomThenDeviceLightingChangedEventBroadcast()
        {
            var hub = await _factory.CreateHubConnection("events");
            HausEvent<DeviceLightingChangedEvent> change = null;
            hub.On<HausEvent<DeviceLightingChangedEvent>>("OnEvent", e => change = e);

            var (_, device) = await _factory.AddRoomWithDevice("my-room", DeviceType.Light);
            
            Eventually.Assert(() =>
            {
                change.Payload.Device.Id.Should().Be(device.Id);
            });
        }
    }
}