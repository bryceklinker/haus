using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Connection;

// Lifecycle/IO plumbing that owns the deCONZ framing envelope on both directions: it appends
// the checksum and SLIP-encodes an outbound command before writing it, then correlates the
// eventual response by the sequence number that every deCONZ frame carries at byte offset 1.
// It has no knowledge of what any individual command means.
public class DeconzChannel(ISerialTransport transport)
{
    private const int SequenceNumberIndex = 1;

    private readonly ISerialTransport _transport = transport;
    private readonly FrameReader _reader = new(transport);
    private readonly SlipEncoder _encoder = new();

    // ZigbeeCoordinator shares one DeconzChannel between its background poll loop and every
    // caller sending a command (APS data requests, permit-join, parameter writes). Without this,
    // two overlapping calls could interleave their writes on the wire, and -- since AwaitResponseAsync
    // discards any frame that doesn't match its own sequence number -- one call's read could
    // silently steal and drop the frame another concurrent call was waiting for, hanging it
    // forever. This serializes the whole request/response round trip, matching the real dongle's
    // single physical serial line anyway.
    private readonly SemaphoreSlim _mutex = new(1, 1);

    public async Task<byte[]> SendAndReceiveAsync(byte[] frame, CancellationToken token)
    {
        await _mutex.WaitAsync(token);
        try
        {
            await _transport.WriteAsync(Encode(frame), token);
            return await AwaitResponseAsync(frame[SequenceNumberIndex], token);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private byte[] Encode(byte[] frame)
    {
        var checksum = DeconzChecksum.Compute(frame);
        var withChecksum = new byte[frame.Length + 2];
        frame.CopyTo(withChecksum, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(withChecksum.AsSpan(frame.Length), checksum);
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
        return [];
    }
}
