using Haus.Zigbee.Host.Zigbee;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee;

public class DeviceAddressRegistryTests
{
    private readonly DeviceAddressRegistry _registry = new();

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
}
