using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;
using Xunit.Abstractions;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class DevicesRealtimeApiTests(HausWebHostApplicationFactory factory, ITestOutputHelper output)
{
    [Fact]
    public async Task WhenLightIsAddedToRoomThenDeviceLightingChangedEventBroadcast()
    {
        var hub = await factory.CreateHubConnection("events");
        HausEvent<DeviceLightingChangedEvent> change = null;
        hub.On<HausEvent<DeviceLightingChangedEvent>>(
            "OnEvent",
            e =>
            {
                if (e?.Payload?.Device != null)
                    change = e;
            }
        );

        var (_, device) = await factory.AddRoomWithDevice("my-room", DeviceType.Light);

        Eventually.Assert(() =>
        {
            output.WriteLine("**************************************");
            output.WriteLine($"{HausJsonSerializer.Serialize(change)}");
            output.WriteLine("**************************************");
            change.Payload.Device.Id.Should().Be(device.Id);
        });
    }
}
