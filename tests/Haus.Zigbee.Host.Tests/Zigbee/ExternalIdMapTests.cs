using Haus.Zigbee;
using Haus.Zigbee.Host.Zigbee;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee;

public class ExternalIdMapTests
{
    [Fact]
    public void ToExternalId_ReturnsLegacyFriendlyNameFormat()
    {
        var address = new IeeeAddress(0x00124b0012345678);

        var externalId = ExternalIdMap.ToExternalId(address);

        Assert.Equal("0x00124b0012345678", externalId);
    }

    [Fact]
    public void TryParseAddress_ValidExternalId_ReturnsTrueAndAddress()
    {
        var found = ExternalIdMap.TryParseAddress("0x00124b0012345678", out var address);

        Assert.True(found);
        Assert.Equal(new IeeeAddress(0x00124b0012345678), address);
    }

    [Fact]
    public void TryParseAddress_InvalidExternalId_ReturnsFalse()
    {
        var found = ExternalIdMap.TryParseAddress("not-an-address", out _);

        Assert.False(found);
    }
}
