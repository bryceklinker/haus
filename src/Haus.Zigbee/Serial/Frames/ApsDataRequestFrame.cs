using System.Collections.Generic;
using Haus.Zigbee;

namespace Haus.Zigbee.Serial.Frames;

public enum ApsAddressMode : byte
{
    Group = 0x01,
    Nwk = 0x02,
    Ieee = 0x03,
}

public sealed record ApsDestination
{
    private ApsDestination(ApsAddressMode mode, ushort shortAddress, IeeeAddress ieeeAddress, byte endpoint)
    {
        Mode = mode;
        ShortAddress = shortAddress;
        IeeeAddress = ieeeAddress;
        Endpoint = endpoint;
    }

    public ApsAddressMode Mode { get; }
    public ushort ShortAddress { get; }
    public IeeeAddress IeeeAddress { get; }
    public byte Endpoint { get; }

    public static ApsDestination Group(ushort groupAddress)
    {
        return new ApsDestination(ApsAddressMode.Group, groupAddress, default, default);
    }
}

public sealed record ApsDataRequestFrame(
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

        var bytes = new List<byte> { CommandId, frame.SequenceNumber, Reserved };
        AddUInt16(bytes, (ushort)frameLength);
        AddUInt16(bytes, (ushort)payloadLength);
        bytes.Add(frame.RequestId);
        bytes.Add(Reserved);
        bytes.Add((byte)frame.Destination.Mode);
        bytes.AddRange(destinationAddress);
        AddUInt16(bytes, frame.ProfileId);
        AddUInt16(bytes, frame.ClusterId);
        bytes.Add(frame.SourceEndpoint);
        AddUInt16(bytes, (ushort)frame.AsduPayload.Length);
        bytes.AddRange(frame.AsduPayload);
        bytes.Add(frame.TxOptions);
        bytes.Add(frame.Radius);
        return bytes.ToArray();
    }

    private static List<byte> EncodeDestination(ApsDestination destination)
    {
        var bytes = new List<byte>();
        AddUInt16(bytes, destination.ShortAddress);
        return bytes;
    }

    private static void AddUInt16(List<byte> bytes, ushort value)
    {
        bytes.Add((byte)(value & 0xff));
        bytes.Add((byte)(value >> 8));
    }
}
