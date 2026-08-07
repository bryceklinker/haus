using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Tests.Transport;

// In-memory ISerialTransport double: a test feeds bytes in (simulating what a real deCONZ
// coordinator would send back) and inspects the bytes that were written out (simulating what
// we sent to the coordinator).
public class FakeSerialTransport : ISerialTransport
{
    private readonly List<byte> _writtenBytes = new();
    private readonly Queue<byte> _incomingBytes = new();

    public IReadOnlyList<byte> WrittenBytes => _writtenBytes;

    public void FeedIncoming(byte[] bytes)
    {
        foreach (var value in bytes)
            _incomingBytes.Enqueue(value);
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
        var count = 0;
        while (count < buffer.Length && _incomingBytes.Count > 0)
            buffer.Span[count++] = _incomingBytes.Dequeue();
        return Task.FromResult(count);
    }

    public void Dispose() { }
}
