using System.Threading.Tasks;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Models;
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
        await _store.UpsertAsync(new RegisterKnownDevice("0x1", 0x1234, [], IsSleepy: true));

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal("0x1", device.ExternalId);
        Assert.Equal((ushort)0x1234, device.NetworkAddress);
        Assert.True(device.IsSleepy);
    }

    [Fact]
    public async Task UpsertAsync_ExistingExternalId_UpdatesTheSameDeviceRatherThanCreatingASecondOne()
    {
        await _store.UpsertAsync(new RegisterKnownDevice("0x1", 0x1234, [], IsSleepy: false));

        var endpoints = new[] { new ZigbeeEndpoint(1, 0x0104, 0x0100, [0x0006], []) };
        await _store.UpsertAsync(new RegisterKnownDevice("0x1", 0x5678, endpoints, IsSleepy: true));

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal((ushort)0x5678, device.NetworkAddress);
        var endpoint = Assert.Single(device.Endpoints);
        Assert.Equal((byte)1, endpoint.EndpointId);
        Assert.Equal(new ushort[] { 0x0006 }, endpoint.InClusters);
        Assert.True(device.IsSleepy);
    }

    [Fact]
    public async Task UpsertNetworkAddressAsync_NewExternalId_CreatesAMinimalDevice()
    {
        await _store.UpsertNetworkAddressAsync(new RegisterNetworkAddress("0x1", 0x1234));

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal("0x1", device.ExternalId);
        Assert.Equal((ushort)0x1234, device.NetworkAddress);
        Assert.False(device.IsSleepy);
    }

    [Fact]
    public async Task UpsertNetworkAddressAsync_ExistingDevice_UpdatesOnlyTheNetworkAddress()
    {
        var endpoints = new[] { new ZigbeeEndpoint(1, 0x0104, 0x0100, [0x0006], []) };
        await _store.UpsertAsync(new RegisterKnownDevice("0x1", 0x1234, endpoints, IsSleepy: true));

        await _store.UpsertNetworkAddressAsync(new RegisterNetworkAddress("0x1", 0x9abc));

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal((ushort)0x9abc, device.NetworkAddress);
        var endpoint = Assert.Single(device.Endpoints);
        Assert.Equal((byte)1, endpoint.EndpointId);
        Assert.Equal(new ushort[] { 0x0006 }, endpoint.InClusters);
        Assert.True(device.IsSleepy);
    }

    [Fact]
    public async Task SeedIfAbsentAsync_NoExistingRow_CreatesADeviceFromTheSeed()
    {
        var endpoints = new[] { new ZigbeeEndpoint(1, 0x0104, 0x0100, [0x0006], []) };
        await _store.SeedIfAbsentAsync(new RegisterKnownDevice("0x1", 0x1234, endpoints, IsSleepy: true));

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal("0x1", device.ExternalId);
        Assert.Equal((ushort)0x1234, device.NetworkAddress);
        Assert.True(device.IsSleepy);
    }

    [Fact]
    public async Task SeedIfAbsentAsync_ExistingRow_LeavesItUnchanged()
    {
        await _store.UpsertAsync(new RegisterKnownDevice("0x1", 0x1234, [], IsSleepy: false));

        await _store.SeedIfAbsentAsync(new RegisterKnownDevice("0x1", 0x9abc, [], IsSleepy: true));

        var device = Assert.Single(await _store.GetAllAsync());
        Assert.Equal((ushort)0x1234, device.NetworkAddress);
        Assert.False(device.IsSleepy);
    }

    [Fact]
    public async Task SeedIfAbsentAsync_RichLiveRecordAlreadyPersisted_IsNotDowngradedBySeed()
    {
        var liveEndpoints = new[] { new ZigbeeEndpoint(1, 0x0104, 0x0100, [0x0006], [0x0019]) };
        await _store.UpsertAsync(new RegisterKnownDevice("0x1", 0x1234, liveEndpoints, IsSleepy: true));

        await _store.SeedIfAbsentAsync(new RegisterKnownDevice("0x1", 0x1234, [], IsSleepy: false));

        var device = Assert.Single(await _store.GetAllAsync());
        var endpoint = Assert.Single(device.Endpoints);
        Assert.Equal(new ushort[] { 0x0019 }, endpoint.OutClusters);
        Assert.True(device.IsSleepy);
    }

    [Fact]
    public async Task SeedIfAbsentAsync_AppliedTwice_LeavesExactlyOneRowForTheExternalId()
    {
        await _store.SeedIfAbsentAsync(new RegisterKnownDevice("0x1", 0x1234, [], IsSleepy: true));
        await _store.SeedIfAbsentAsync(new RegisterKnownDevice("0x1", 0x1234, [], IsSleepy: true));

        Assert.Single(await _store.GetAllAsync());
    }
}
