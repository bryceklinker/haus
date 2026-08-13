using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Models;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee;

public class ExternalIdConverterTests
{
    [Fact]
    public void ToExternalId_ReturnsLegacyFriendlyNameFormat()
    {
        var address = new IeeeAddress(0x00124b0012345678);

        var externalId = ExternalIdConverter.ToExternalId(address);

        Assert.Equal("0x00124b0012345678", externalId);
    }

    [Fact]
    public void TryParseAddress_ValidExternalId_ReturnsTrueAndAddress()
    {
        var found = ExternalIdConverter.TryParseAddress("0x00124b0012345678", out var address);

        Assert.True(found);
        Assert.Equal(new IeeeAddress(0x00124b0012345678), address);
    }

    [Fact]
    public void TryParseAddress_InvalidExternalId_ReturnsFalse()
    {
        var found = ExternalIdConverter.TryParseAddress("not-an-address", out _);

        Assert.False(found);
    }
}
