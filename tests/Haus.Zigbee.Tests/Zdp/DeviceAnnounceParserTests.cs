using Haus.Zigbee.Models;
using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Zdp;

public class DeviceAnnounceParserTests
{
    [Fact]
    public void WhenPayloadDecodedThenRecoversTransactionSequenceNumberNetworkAddressIeeeAddressAndCapabilities()
    {
        var payload = new byte[]
        {
            0x2a, // TSN
            0xb2,
            0xa1, // NWKAddrOfInterest (u16 LE) => 0xa1b2
            0x67,
            0x45,
            0x23,
            0x01,
            0x00,
            0x4b,
            0x12,
            0x00, // IEEEAddr (u64 LE) => 0x00124b0001234567
            0x8e, // Capabilities
        };

        var announce = DeviceAnnounceParser.Parse(payload);

        Assert.Equal((byte)0x2a, announce.TransactionSequenceNumber);
        Assert.Equal((ushort)0xa1b2, announce.NetworkAddress);
        Assert.Equal(new IeeeAddress(0x00124b0001234567), announce.IeeeAddress);
        Assert.Equal((byte)0x8e, announce.Capabilities);
    }
}
