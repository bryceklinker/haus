using System.Collections.Generic;
using System.Linq;
using Haus.Zigbee.Serial;

namespace Haus.Zigbee.Tests.Coordinator;

internal sealed record IndicationBody(
    ushort SourceNwk,
    byte SourceEndpoint,
    ushort ProfileId,
    ushort ClusterId,
    byte[] Asdu
);

internal static class DeconzFrames
{
    private const byte DeviceStateCommand = 0x07;
    private const byte ReadIndicationCommand = 0x17;
    private const byte ApsDataRequestCommand = 0x12;
    private const byte SuccessStatus = 0x00;
    private const byte NwkAddressMode = 0x02;
    private const byte NoRssiLinkQuality = 0xff;

    public static byte[] DeviceState(byte sequenceNumber, byte deviceState)
    {
        return Framed(new byte[] { DeviceStateCommand, sequenceNumber, 0x00, 0x00, 0x00, deviceState });
    }

    public static byte[] RequestAck(byte sequenceNumber)
    {
        return Framed(new byte[] { ApsDataRequestCommand, sequenceNumber, SuccessStatus, 0x00, 0x00 });
    }

    public static byte[] Indication(byte sequenceNumber, IndicationBody body)
    {
        var header = new byte[] { ReadIndicationCommand, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var destination = new byte[] { NwkAddressMode, 0x00, 0x00, 0x01 };
        var source = new byte[]
        {
            NwkAddressMode,
            (byte)(body.SourceNwk & 0xff),
            (byte)(body.SourceNwk >> 8),
            body.SourceEndpoint,
        };
        var profileAndCluster = new byte[]
        {
            (byte)(body.ProfileId & 0xff),
            (byte)(body.ProfileId >> 8),
            (byte)(body.ClusterId & 0xff),
            (byte)(body.ClusterId >> 8),
        };
        var asdu = Concat(new byte[] { (byte)(body.Asdu.Length & 0xff), (byte)(body.Asdu.Length >> 8) }, body.Asdu);
        var reservedAndLinkQuality = new byte[] { 0x00, 0x00, NoRssiLinkQuality };
        return Framed(Concat(header, destination, source, profileAndCluster, asdu, reservedAndLinkQuality));
    }

    private static byte[] Concat(params byte[][] segments)
    {
        return segments.SelectMany(segment => segment).ToArray();
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
