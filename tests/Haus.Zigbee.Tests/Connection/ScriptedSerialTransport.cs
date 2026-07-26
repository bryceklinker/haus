using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Tests.Connection;

// Test transport that models a real deCONZ coordinator answering one command at a time: each
// read returns the bytes of exactly one queued response frame, so a caller that issues several
// sequential command round-trips gets one response per round-trip instead of having every
// queued response drained into the first read.
internal class ScriptedSerialTransport : ISerialTransport
{
    private readonly List<byte> _writtenBytes = new();
    private readonly Queue<byte[]> _responses = new();

    public IReadOnlyList<byte> WrittenBytes => _writtenBytes;

    public void QueueResponse(byte[] frameBytes)
    {
        _responses.Enqueue(frameBytes);
    }

    public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

    public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
    {
        _writtenBytes.AddRange(buffer.ToArray());
        return Task.CompletedTask;
    }

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
    {
        if (_responses.Count == 0)
            return Task.FromResult(0);

        var response = _responses.Dequeue();
        response.CopyTo(buffer);
        return Task.FromResult(response.Length);
    }
}
