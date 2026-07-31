using Haus.Zigbee.Models;
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

    [Fact]
    public void WhenParsedFromStringMissingPrefixThenParsingFails()
    {
        var parsed = IeeeAddress.TryParse("00124b0001234567", out _);

        Assert.False(parsed);
    }

    [Fact]
    public void WhenParsedFromStringWithFewerThanSixteenHexDigitsThenParsingFails()
    {
        var parsed = IeeeAddress.TryParse("0x123", out _);

        Assert.False(parsed);
    }

    [Theory]
    [InlineData("0x0000000000000001")]
    [InlineData("0x00124b0001234567")]
    [InlineData("0xffffffffffffffff")]
    public void WhenParsedAndFormattedBackThenReturnsTheOriginalCanonicalString(string canonical)
    {
        IeeeAddress.TryParse(canonical, out var address);

        Assert.Equal(canonical, address.ToString());
    }
}
