using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Tests.Coordinator;

// Adapts the simulator's DeconzResponder -- which already answers device-state polls, parameter
// reads, and APS data-requests (auto-queuing the matching confirm) exactly like a real dongle --
// to the ISerialTransport seam, so a coordinator-level test can drive a fully confirmed send
// without re-implementing that protocol logic a third time.
internal class ResponderBackedTransport(DeconzResponder responder) : ISerialTransport
{
    private const int ChecksumLength = 2;

    private byte[] _pendingResponse = Array.Empty<byte>();

    public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

    public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
    {
        _pendingResponse = DeconzFrames.Framed(responder.HandleRequest(DecodeRequest(buffer.ToArray())));
        return Task.CompletedTask;
    }

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
    {
        var response = _pendingResponse;
        _pendingResponse = Array.Empty<byte>();
        response.CopyTo(buffer);
        return Task.FromResult(response.Length);
    }

    public void Dispose() { }

    private static byte[] DecodeRequest(byte[] framed)
    {
        var frame = new SlipDecoder().Decode(framed)[0];
        return frame[..^ChecksumLength];
    }
}
