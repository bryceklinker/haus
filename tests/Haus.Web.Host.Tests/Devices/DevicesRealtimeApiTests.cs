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
public class DevicesRealtimeApiTests
{
    private readonly HausWebHostApplicationFactory _factory;
    private readonly ITestOutputHelper _output;

    public DevicesRealtimeApiTests(HausWebHostApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    [Fact]
    public async Task WhenLightIsAddedToRoomThenDeviceLightingChangedEventBroadcast()
    {
        var hub = await _factory.CreateHubConnection("events");
        HausEvent<DeviceLightingChangedEvent> change = null;
        hub.On<HausEvent<DeviceLightingChangedEvent>>("OnEvent", e =>
        {
            if (e?.Payload?.Device != null) change = e;
        });

        var (_, device) = await _factory.AddRoomWithDevice("my-room", DeviceType.Light);

        Eventually.Assert(() =>
        {
            _output.WriteLine("**************************************");
            _output.WriteLine($"{HausJsonSerializer.Serialize(change)}");
            _output.WriteLine("**************************************");
            change.Payload.Device.Id.Should().Be(device.Id);
        });
    }
}