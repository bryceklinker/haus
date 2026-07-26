using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Connection;

// Lifecycle/IO plumbing that owns the deCONZ framing envelope on both directions: it appends
// the checksum and SLIP-encodes an outbound command before writing it, then correlates the
// eventual response by the sequence number that every deCONZ frame carries at byte offset 1.
// It has no knowledge of what any individual command means.
public class DeconzChannel
{
    private const int SequenceNumberIndex = 1;

    private readonly ISerialTransport _transport;
    private readonly FrameReader _reader;
    private readonly SlipEncoder _encoder = new();

    public DeconzChannel(ISerialTransport transport)
    {
        _transport = transport;
        _reader = new FrameReader(transport);
    }

    public async Task<byte[]> SendAndReceiveAsync(byte[] frame, CancellationToken token)
    {
        await _transport.WriteAsync(Encode(frame), token);
        return await AwaitResponseAsync(frame[SequenceNumberIndex], token);
    }

    private byte[] Encode(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new byte[frame.Length + 2];
        frame.CopyTo(withChecksum, 0);
        withChecksum[^2] = (byte)(checksum & 0xff);
        withChecksum[^1] = (byte)(checksum >> 8);
        return _encoder.Encode(withChecksum);
    }

    private async Task<byte[]> AwaitResponseAsync(byte sequenceNumber, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var frames = await _reader.ReadFramesAsync(token);
            foreach (var frame in frames)
                if (frame[SequenceNumberIndex] == sequenceNumber)
                    return frame;
        }
        return Array.Empty<byte>();
    }
}
