using Haus.Zigbee;
using Xunit;

namespace Haus.Zigbee.Tests;

public class IeeeAddressTests
{
    [Fact]
    public void WhenFormattedThenReturnsZeroXPrefixedLowercaseSixteenDigitHex()
    {
        var address = new IeeeAddress(0x00124b0001234567);

        Assert.Equal("0x00124b0001234567", address.ToString());
    }

    [Fact]
    public void WhenTwoAddressesHaveTheSameValueThenTheyAreEqual()
    {
        var address = new IeeeAddress(0x00124b0001234567);
        var sameAddress = new IeeeAddress(0x00124b0001234567);

        Assert.Equal(address, sameAddress);
    }

    [Fact]
    public void WhenParsedFromCanonicalStringThenReturnsAddressWithThatValue()
    {
        var parsed = IeeeAddress.TryParse("0x00124b0001234567", out var address);

        Assert.True(parsed);
        Assert.Equal(new IeeeAddress(0x00124b0001234567), address);
    }
}
