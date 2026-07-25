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
}
