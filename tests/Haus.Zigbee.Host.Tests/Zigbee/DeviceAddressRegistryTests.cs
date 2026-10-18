using System.Threading.Tasks;
using Haus.Testing.Support;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee;

public class DeviceAddressRegistryTests
{
    private readonly IKnownZigbeeDeviceStore _store;
    private readonly DeviceAddressRegistry _registry;

    public DeviceAddressRegistryTests()
    {
        var provider = ServiceProviderFactory.Create();
        _store = provider.GetRequiredService<IKnownZigbeeDeviceStore>();
        _registry = provider.GetRequiredService<DeviceAddressRegistry>();
    }

    [Fact]
    public void TryGetExternalId_UnknownNetworkAddress_ReturnsFalse()
    {
        var found = _registry.TryGetExternalId(0x1234, out _);

        Assert.False(found);
    }

    [Fact]
    public void TryGetExternalId_RegisteredNetworkAddress_ReturnsTrueAndExternalId()
    {
        _registry.Register(0x1234, "0x00124b0012345678");

        var found = _registry.TryGetExternalId(0x1234, out var externalId);

        Assert.True(found);
        Assert.Equal("0x00124b0012345678", externalId);
    }

    [Fact]
    public void Register_SameNetworkAddressTwice_LatestExternalIdWins()
    {
        _registry.Register(0x1234, "0xold");
        _registry.Register(0x1234, "0xnew");

        _registry.TryGetExternalId(0x1234, out var externalId);

        Assert.Equal("0xnew", externalId);
    }

    [Fact]
    public async Task Register_PersistsTheNetworkAddressForFutureRestarts()
    {
        _registry.Register(0x1234, "0x00124b0012345678");

        await Eventually.AssertAsync(async () =>
        {
            var devices = await _store.GetAllAsync();
            var device = Assert.Single(devices);
            Assert.Equal("0x00124b0012345678", device.ExternalId);
            Assert.Equal((ushort)0x1234, device.NetworkAddress);
        });
    }

    [Fact]
    public async Task RegisterKnownDevice_PersistsEndpointsAndSleepyState()
    {
        var address = new IeeeAddress(1);
        var endpoints = new[] { new ZigbeeEndpoint(1, 0x0104, 0x0100, [0x0000, 0x0006], [0x0019]) };

        _registry.RegisterKnownDevice(address, 0x1234, endpoints, isSleepy: true);

        await Eventually.AssertAsync(async () =>
        {
            var devices = await _store.GetAllAsync();
            var device = Assert.Single(devices);
            Assert.Equal(ExternalIdConverter.ToExternalId(address), device.ExternalId);
            Assert.True(device.IsSleepy);
            Assert.Contains("\"EndpointId\":1", device.EndpointsJson);
        });
    }

    [Fact]
    public async Task LoadAsync_PrimesTheCacheFromAlreadyPersistedDevices_SoAFreshInstanceResolvesWithoutReregistering()
    {
        var address = new IeeeAddress(1);
        _registry.RegisterKnownDevice(address, 0x1234, [], isSleepy: false);
        await Eventually.AssertAsync(async () => Assert.Single(await _store.GetAllAsync()));

        var registryAfterRestart = new DeviceAddressRegistry(_store, NullLogger<DeviceAddressRegistry>.Instance);
        await registryAfterRestart.LoadAsync();

        var found = registryAfterRestart.TryGetExternalId(0x1234, out var externalId);
        Assert.True(found);
        Assert.Equal(ExternalIdConverter.ToExternalId(address), externalId);
    }
}
