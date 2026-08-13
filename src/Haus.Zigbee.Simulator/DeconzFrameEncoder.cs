using System.Linq;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Simulator;

// Builds the unframed, unchecksummed bytes for deCONZ ReadIndication/ReadConfirm responses the
// same way a real coordinator dongle would encode them on the wire. The connection loop owns
// SLIP/checksum framing on top of what this produces.
public static class DeconzFrameEncoder
{
    public const byte ReadIndicationCommand = 0x17;
    public const byte ReadConfirmCommand = 0x04;
    public const byte SuccessStatus = 0x00;

    // DeviceState/ReadIndication/ApsDataRequest-ack responses all carry a frameLength (and, for
    // ReadIndication, a payloadLength) field that the real client-side decoder never validates,
    // so these are left zero rather than computed.
    public const ushort UnvalidatedFrameLength = 0;
    public const ushort UnvalidatedPayloadLength = 0;

    // Indications are always addressed to the coordinator itself: network address 0x0000 on its
    // own listening endpoint 1.
    private const ushort CoordinatorNetworkAddress = 0x0000;
    private const byte CoordinatorEndpoint = 0x01;
    private const byte NoRssiLinkQuality = 0xff;

    public static byte[] EncodeIndication(byte sequenceNumber, IndicationBody body)
    {
        var header = BuildUnvalidatedHeader(ReadIndicationCommand, sequenceNumber);
        var destination = Concat(
            [(byte)DeconzAddressMode.Nwk],
            LittleEndian.Bytes(CoordinatorNetworkAddress),
            [CoordinatorEndpoint]
        );
        byte[] source = [(byte)DeconzAddressMode.Nwk, .. LittleEndian.Bytes(body.SourceNwk), body.SourceEndpoint];
        byte[] profileAndCluster = [.. LittleEndian.Bytes(body.ProfileId), .. LittleEndian.Bytes(body.ClusterId)];
        var asdu = Concat(LittleEndian.Bytes((ushort)body.Asdu.Length), body.Asdu);
        byte[] reservedAndLinkQuality = [0x00, 0x00, NoRssiLinkQuality];
        return Concat(header, destination, source, profileAndCluster, asdu, reservedAndLinkQuality);
    }

    public static byte[] EncodeConfirm(byte sequenceNumber, ConfirmBody body)
    {
        var header = Concat(BuildUnvalidatedHeader(ReadConfirmCommand, sequenceNumber), [body.RequestId]);
        var destination = Concat(
            [(byte)DeconzAddressMode.Nwk],
            LittleEndian.Bytes(body.DestinationNetworkAddress),
            [body.DestinationEndpoint]
        );
        byte[] sourceEndpointAndStatus = [body.SourceEndpoint, SuccessStatus];
        return Concat(header, destination, sourceEndpointAndStatus);
    }

    // The frameLength/payloadLength fields are never validated by the real client-side decoder, and
    // the trailing device-state byte is captured but nothing downstream reads it -- it's a distinct
    // field from (and unrelated to) the DeviceState poll response's own available-flags.
    private static byte[] BuildUnvalidatedHeader(byte command, byte sequenceNumber)
    {
        const byte unusedDeviceState = 0x00;
        return Concat(
            [command, sequenceNumber, SuccessStatus],
            LittleEndian.Bytes(UnvalidatedFrameLength),
            LittleEndian.Bytes(UnvalidatedPayloadLength),
            [unusedDeviceState]
        );
    }

    private static byte[] Concat(params byte[][] segments)
    {
        return segments.SelectMany(segment => segment).ToArray();
    }
}
