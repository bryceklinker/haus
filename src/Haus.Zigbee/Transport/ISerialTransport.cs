using System;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Zigbee.Transport;

// The single injectable seam for the whole library: every layer above the serial line is
// driven through this port so tests never touch real hardware.
public interface ISerialTransport
{
    Task OpenAsync(CancellationToken token);

    Task CloseAsync(CancellationToken token);

    Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token);

    Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token);
}
