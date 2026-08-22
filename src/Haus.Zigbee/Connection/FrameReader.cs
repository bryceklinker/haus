using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Haus.Zigbee.Connection;

// Lifecycle/IO plumbing: pulls raw bytes off the transport, reassembles SLIP frames across
// reads, and surfaces only checksum-valid frames with their trailing checksum stripped.
// It knows nothing about deCONZ command semantics.
public class FrameReader(ISerialTransport transport, ILogger<FrameReader>? logger = null)
{
    private const int ReadBufferSize = 256;
    private const int SequenceNumberIndex = 1;

    private readonly ISerialTransport _transport = transport;
    private readonly SlipDecoder _decoder = new();
    private readonly ILogger<FrameReader> _logger = logger ?? NullLogger<FrameReader>.Instance;

    public async Task<IReadOnlyList<byte[]>> ReadFramesAsync(CancellationToken token)
    {
        var buffer = new byte[ReadBufferSize];
        var count = await _transport.ReadAsync(buffer, token);
        return ValidatedFrames(_decoder.Decode(buffer[..count]));
    }

    private IReadOnlyList<byte[]> ValidatedFrames(IReadOnlyList<byte[]> decodedFrames)
    {
        var validated = new List<byte[]>();
        foreach (var frame in decodedFrames)
        {
            if (!DeconzChecksum.IsValid(frame))
            {
                _logger.LogWarning("Discarding frame with an invalid checksum: {@Frame}", frame);
                continue;
            }

            var stripped = frame[..^2];
            _logger.LogDebug(
                "Read frame with sequence number {@SequenceNumber}: {@Frame}",
                stripped.Length > SequenceNumberIndex ? stripped[SequenceNumberIndex] : (byte?)null,
                stripped
            );
            validated.Add(stripped);
        }
        return validated;
    }
}
