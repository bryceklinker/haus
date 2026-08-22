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
public class DeconzChannel(ISerialTransport transport, TimeSpan? roundTripTimeout = null)
{
    private const int SequenceNumberIndex = 1;

    // Linux's System.IO.Ports.SerialPort can leave a write or read against a dropped/torn-down
    // port hanging forever instead of throwing, so a timeout alone can't rely on the awaited
    // Task ever completing on its own -- only disposing the transport can force it to fault.
    private static readonly TimeSpan DefaultRoundTripTimeout = TimeSpan.FromSeconds(5);

    private readonly ISerialTransport _transport = transport;
    private readonly FrameReader _reader = new(transport);
    private readonly SlipEncoder _encoder = new();
    private readonly TimeSpan _roundTripTimeout = roundTripTimeout ?? DefaultRoundTripTimeout;

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
            return await SendAndReceiveWithTimeoutAsync(frame, token);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private async Task<byte[]> SendAndReceiveWithTimeoutAsync(byte[] frame, CancellationToken token)
    {
        var roundTrip = RoundTripAsync(frame, token);
        var timeout = Task.Delay(_roundTripTimeout, token);
        var finished = await Task.WhenAny(roundTrip, timeout);
        if (finished == roundTrip)
            return await roundTrip;

        // Task.Delay observes the caller's token too, so it can also be the one that "won" the
        // race because the caller cancelled -- that's not necessarily a hang. But it isn't
        // necessarily a healthy transport either: Linux's SerialPort doesn't reliably honor
        // CancellationToken, so the abandoned round trip can still be silently wedged. We can't
        // just let it go -- this method returning is what lets the mutex's finally release, and
        // handing the mutex to the next caller while this round trip might still be stuck against
        // the transport underneath it is exactly the interleaved-I/O hazard the mutex exists to
        // prevent. So even on caller cancellation, we must find out whether the transport is
        // actually still alive before returning control.
        if (token.IsCancellationRequested)
        {
            await ResolveAbandonedRoundTripAsync(roundTrip);
            throw new OperationCanceledException(token);
        }

        _transport.Dispose();
        ObserveFault(roundTrip);
        throw new SerialTransportTimeoutException(_roundTripTimeout);
    }

    private async Task ResolveAbandonedRoundTripAsync(Task<byte[]> roundTrip)
    {
        var grace = Task.Delay(_roundTripTimeout, CancellationToken.None);
        var finished = await Task.WhenAny(roundTrip, grace);
        if (finished == roundTrip)
            return;

        _transport.Dispose();
        ObserveFault(roundTrip);
    }

    private async Task<byte[]> RoundTripAsync(byte[] frame, CancellationToken token)
    {
        await _transport.WriteAsync(Encode(frame), token);
        return await AwaitResponseAsync(frame[SequenceNumberIndex], token);
    }

    // The disposed transport eventually faults this task's pending write/read; nothing else
    // observes that fault, so leaving it unawaited would otherwise surface as an unobserved
    // task exception later.
    private static void ObserveFault(Task task) =>
        task.ContinueWith(
            completed => _ = completed.Exception,
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default
        );

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
        while (true)
        {
            var frames = await _reader.ReadFramesAsync(token);
            foreach (var frame in frames)
                if (frame[SequenceNumberIndex] == sequenceNumber)
                    return frame;
            token.ThrowIfCancellationRequested();
        }
    }
}
