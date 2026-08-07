using System.Collections.Generic;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Simulator;

namespace Haus.Zigbee.Tests.Coordinator;

internal static class DeconzFrames
{
    private const byte DeviceStateCommand = 0x07;
    private const byte ApsDataRequestCommand = 0x12;
    private const byte SuccessStatus = 0x00;

    public static byte[] DeviceState(byte sequenceNumber, byte deviceState)
    {
        return Framed(new byte[] { DeviceStateCommand, sequenceNumber, 0x00, 0x00, 0x00, deviceState });
    }

    public static byte[] RequestAck(byte sequenceNumber)
    {
        return Framed(new byte[] { ApsDataRequestCommand, sequenceNumber, SuccessStatus, 0x00, 0x00 });
    }

    // Reuses the Simulator's own indication encoder rather than keeping a second copy of the
    // deCONZ indication wire layout in test code -- the two must otherwise change together.
    public static byte[] Indication(byte sequenceNumber, IndicationBody body)
    {
        return Framed(DeconzResponder.EncodeIndication(sequenceNumber, body));
    }

    public static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzChecksum.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
