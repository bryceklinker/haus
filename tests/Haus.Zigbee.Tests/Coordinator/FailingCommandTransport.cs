using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Tests.Coordinator;

internal class FailingCommandTransport(ISerialTransport inner, byte failingCommandId, Exception exception)
    : ISerialTransport
{
    public Task OpenAsync(CancellationToken token) => inner.OpenAsync(token);

    public Task CloseAsync(CancellationToken token) => inner.CloseAsync(token);

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
    {
        var frame = new SlipDecoder().Decode(buffer.ToArray())[0];
        if (frame[0] == failingCommandId)
            throw exception;
        return inner.WriteAsync(buffer, token);
    }

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token) => inner.ReadAsync(buffer, token);

    public void Dispose() => inner.Dispose();
}
