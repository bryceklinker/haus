using System.Collections.Generic;
using Haus.Zigbee.Models;

namespace Haus.Zigbee.Serial.Frames;

public record ApsDestination
{
    private ApsDestination(DeconzAddressMode mode, ushort shortAddress, IeeeAddress ieeeAddress, byte endpoint)
    {
        Mode = mode;
        ShortAddress = shortAddress;
        IeeeAddress = ieeeAddress;
        Endpoint = endpoint;
    }

    public DeconzAddressMode Mode { get; }
    public ushort ShortAddress { get; }
    public IeeeAddress IeeeAddress { get; }
    public byte Endpoint { get; }

    public static ApsDestination Group(ushort groupAddress)
    {
        return new ApsDestination(DeconzAddressMode.Group, groupAddress, default, default);
    }

    public static ApsDestination Nwk(ushort networkAddress, byte endpoint)
    {
        return new ApsDestination(DeconzAddressMode.Nwk, networkAddress, default, endpoint);
    }

    public static ApsDestination Ieee(IeeeAddress ieeeAddress, byte endpoint)
    {
        return new ApsDestination(DeconzAddressMode.Ieee, default, ieeeAddress, endpoint);
    }
}

public record ApsDataRequestFrame(
    byte SequenceNumber,
    byte RequestId,
    ApsDestination Destination,
    ushort ProfileId,
    ushort ClusterId,
    byte SourceEndpoint,
    byte[] AsduPayload,
    byte TxOptions,
    byte Radius
);

public static class ApsDataRequestFrameCodec
{
    private const byte CommandId = 0x12;
    private const byte Reserved = 0x00;
    private const int FixedPayloadBytes = 12;
    private const int FrameLengthOverhead = 7;

    public static byte[] Encode(ApsDataRequestFrame frame)
    {
        var destinationAddress = EncodeDestination(frame.Destination);
        var payloadLength = FixedPayloadBytes + destinationAddress.Count + frame.AsduPayload.Length;
        var frameLength = FrameLengthOverhead + payloadLength;

        List<byte> bytes = [CommandId, frame.SequenceNumber, Reserved];
        LittleEndian.Write(bytes, (ushort)frameLength);
        LittleEndian.Write(bytes, (ushort)payloadLength);
        bytes.Add(frame.RequestId);
        bytes.Add(Reserved);
        bytes.Add((byte)frame.Destination.Mode);
        bytes.AddRange(destinationAddress);
        LittleEndian.Write(bytes, frame.ProfileId);
        LittleEndian.Write(bytes, frame.ClusterId);
        bytes.Add(frame.SourceEndpoint);
        LittleEndian.Write(bytes, (ushort)frame.AsduPayload.Length);
        bytes.AddRange(frame.AsduPayload);
        bytes.Add(frame.TxOptions);
        bytes.Add(frame.Radius);
        return bytes.ToArray();
    }

    private static List<byte> EncodeDestination(ApsDestination destination)
    {
        return destination.Mode switch
        {
            DeconzAddressMode.Group => EncodeGroup(destination),
            DeconzAddressMode.Nwk => EncodeNwk(destination),
            _ => EncodeIeee(destination),
        };
    }

    private static List<byte> EncodeGroup(ApsDestination destination)
    {
        List<byte> bytes = [];
        LittleEndian.Write(bytes, destination.ShortAddress);
        return bytes;
    }

    private static List<byte> EncodeNwk(ApsDestination destination)
    {
        List<byte> bytes = [];
        LittleEndian.Write(bytes, destination.ShortAddress);
        bytes.Add(destination.Endpoint);
        return bytes;
    }

    private static List<byte> EncodeIeee(ApsDestination destination)
    {
        List<byte> bytes = [];
        LittleEndian.Write(bytes, destination.IeeeAddress.Value);
        bytes.Add(destination.Endpoint);
        return bytes;
    }
}
