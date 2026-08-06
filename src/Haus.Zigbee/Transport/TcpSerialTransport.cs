using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Zigbee.Transport;

// A second ISerialTransport implementation for environments with no physical deCONZ dongle
// attached (CI, containers, most dev machines): connects to a TCP endpoint that speaks the exact
// same SLIP+deCONZ byte protocol a real serial line would carry, typically a simulator.
public class TcpSerialTransport(string host, int port) : ISerialTransport
{
    private TcpClient? _client;

    public async Task OpenAsync(CancellationToken token)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port, token);
    }

    public Task CloseAsync(CancellationToken token)
    {
        _client?.Close();
        return Task.CompletedTask;
    }

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) =>
        RequireOpenClient().GetStream().WriteAsync(buffer, token).AsTask();

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token) =>
        RequireOpenClient().GetStream().ReadAsync(buffer, token).AsTask();

    public void Dispose() => _client?.Dispose();

    private TcpClient RequireOpenClient() =>
        _client ?? throw new InvalidOperationException("OpenAsync must be called before reading or writing.");
}
