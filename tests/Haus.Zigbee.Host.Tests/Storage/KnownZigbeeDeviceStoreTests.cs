using System.Threading.Tasks;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Storage;

public class KnownZigbeeDeviceStoreTests
{
    private readonly IKnownZigbeeDeviceStore _store;

    public KnownZigbeeDeviceStoreTests()
    {
        var provider = ServiceProviderFactory.Create();
        _store = provider.GetRequiredService<IKnownZigbeeDeviceStore>();
    }

    [Fact]
    public async Task GetAllAsync_NoDevices_ReturnsEmpty()
    {
        var devices = await _store.GetAllAsync();

        Assert.Empty(devices);
    }

    [Fact]
    public async Task UpsertAsync_NewExternalId_CreatesADevice()
    {
        await _store.UpsertAsync("0x1", 0x1234, "[]", isSleepy: true);

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal("0x1", device.ExternalId);
        Assert.Equal((ushort)0x1234, device.NetworkAddress);
        Assert.True(device.IsSleepy);
    }

    [Fact]
    public async Task UpsertAsync_ExistingExternalId_UpdatesTheSameDeviceRatherThanCreatingASecondOne()
    {
        await _store.UpsertAsync("0x1", 0x1234, "[]", isSleepy: false);

        await _store.UpsertAsync("0x1", 0x5678, "[1]", isSleepy: true);

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal((ushort)0x5678, device.NetworkAddress);
        Assert.Equal("[1]", device.EndpointsJson);
        Assert.True(device.IsSleepy);
    }

    [Fact]
    public async Task UpsertNetworkAddressAsync_NewExternalId_CreatesAMinimalDevice()
    {
        await _store.UpsertNetworkAddressAsync("0x1", 0x1234);

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal("0x1", device.ExternalId);
        Assert.Equal((ushort)0x1234, device.NetworkAddress);
        Assert.False(device.IsSleepy);
    }

    [Fact]
    public async Task UpsertNetworkAddressAsync_ExistingDevice_UpdatesOnlyTheNetworkAddress()
    {
        await _store.UpsertAsync("0x1", 0x1234, "[1]", isSleepy: true);

        await _store.UpsertNetworkAddressAsync("0x1", 0x9abc);

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal((ushort)0x9abc, device.NetworkAddress);
        Assert.Equal("[1]", device.EndpointsJson);
        Assert.True(device.IsSleepy);
    }
}
