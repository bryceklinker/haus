using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Connection;

// Lifecycle/IO plumbing: pulls raw bytes off the transport, reassembles SLIP frames across
// reads, and surfaces only checksum-valid frames with their trailing checksum stripped.
// It knows nothing about deCONZ command semantics.
public class FrameReader
{
    private const int ReadBufferSize = 256;

    private readonly ISerialTransport _transport;
    private readonly SlipDecoder _decoder = new();

    public FrameReader(ISerialTransport transport)
    {
        _transport = transport;
    }

    public async Task<IReadOnlyList<byte[]>> ReadFramesAsync(CancellationToken token)
    {
        var buffer = new byte[ReadBufferSize];
        var count = await _transport.ReadAsync(buffer, token);
        return ValidatedFrames(_decoder.Decode(buffer[..count]));
    }

    private static IReadOnlyList<byte[]> ValidatedFrames(IReadOnlyList<byte[]> decodedFrames)
    {
        var validated = new List<byte[]>();
        foreach (var frame in decodedFrames)
            if (DeconzChecksum.IsValid(frame))
                validated.Add(frame[..^2]);
        return validated;
    }
}
