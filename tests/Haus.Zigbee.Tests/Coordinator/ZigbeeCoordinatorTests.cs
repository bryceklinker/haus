using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Coordinator;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class ZigbeeCoordinatorTests
{
    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1C;

    private readonly FakeDeconzCoordinator _dongle = new();

    [Fact]
    public async Task WhenConnectedThenItReportsTheNetworkConfigReadFromTheCoordinator()
    {
        _dongle.SetParameter(MacAddressParameterId, new byte[] { 0x22, 0x11, 0x00, 0xff, 0xff, 0x2e, 0x21, 0x00 });
        _dongle.SetParameter(PanIdParameterId, new byte[] { 0x62, 0x1a });
        _dongle.SetParameter(ChannelParameterId, new byte[] { 0x0f });
        using var coordinator = new ZigbeeCoordinator(_dongle);

        await coordinator.ConnectAsync(CancellationToken.None);

        Assert.True(coordinator.IsConnected);
        var config = coordinator.NetworkConfig;
        Assert.NotNull(config);
        Assert.Equal(new IeeeAddress(0x00212effff001122), config.MacAddress);
        Assert.Equal((ushort)0x1a62, config.PanId);
        Assert.Equal((byte)0x0f, config.Channel);
    }

    [Fact]
    public async Task WhenNoDeviceHasJoinedThenGetDevicesReturnsAnEmptyList()
    {
        using var coordinator = new ZigbeeCoordinator(_dongle);

        var devices = await coordinator.GetDevicesAsync(CancellationToken.None);

        Assert.Empty(devices);
    }
}
