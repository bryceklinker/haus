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

    public static ApsDestination Nwk(ushort networkAddress, byte endpoint)
    {
        return new ApsDestination(ApsAddressMode.Nwk, networkAddress, default, endpoint);
    }

    public static ApsDestination Ieee(IeeeAddress ieeeAddress, byte endpoint)
    {
        return new ApsDestination(ApsAddressMode.Ieee, default, ieeeAddress, endpoint);
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
        return destination.Mode switch
        {
            ApsAddressMode.Group => EncodeGroup(destination),
            ApsAddressMode.Nwk => EncodeNwk(destination),
            _ => EncodeIeee(destination),
        };
    }

    private static List<byte> EncodeGroup(ApsDestination destination)
    {
        var bytes = new List<byte>();
        AddUInt16(bytes, destination.ShortAddress);
        return bytes;
    }

    private static List<byte> EncodeNwk(ApsDestination destination)
    {
        var bytes = new List<byte>();
        AddUInt16(bytes, destination.ShortAddress);
        bytes.Add(destination.Endpoint);
        return bytes;
    }

    private static List<byte> EncodeIeee(ApsDestination destination)
    {
        var bytes = new List<byte>();
        AddUInt64(bytes, destination.IeeeAddress.Value);
        bytes.Add(destination.Endpoint);
        return bytes;
    }

    private static void AddUInt16(List<byte> bytes, ushort value)
    {
        bytes.Add((byte)(value & 0xff));
        bytes.Add((byte)(value >> 8));
    }

    private static void AddUInt64(List<byte> bytes, ulong value)
    {
        for (var shift = 0; shift < 64; shift += 8)
            bytes.Add((byte)((value >> shift) & 0xff));
    }
}
