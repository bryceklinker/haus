using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Haus.Zigbee.Transport;

// A second ISerialTransport implementation for environments with no physical deCONZ dongle
// attached (CI, containers, most dev machines): connects to a TCP endpoint that speaks the exact
// same SLIP+deCONZ byte protocol a real serial line would carry, typically a simulator.
public class TcpSerialTransport(string host, int port, ILogger<TcpSerialTransport>? logger = null) : ISerialTransport
{
    private readonly ILogger<TcpSerialTransport> _logger = logger ?? NullLogger<TcpSerialTransport>.Instance;
    private TcpClient? _client;

    public async Task OpenAsync(CancellationToken token)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(host, port, token);
        _logger.LogInformation("Connected to {@Host}:{@Port}", host, port);
    }

    public Task CloseAsync(CancellationToken token)
    {
        _client?.Close();
        _logger.LogInformation("Closed connection to {@Host}:{@Port}", host, port);
        return Task.CompletedTask;
    }

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) =>
        WithConnectionErrorLogging(() => RequireOpenClient().GetStream().WriteAsync(buffer, token).AsTask());

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token) =>
        WithConnectionErrorLogging(() => RequireOpenClient().GetStream().ReadAsync(buffer, token).AsTask());

    public void Dispose() => _client?.Dispose();

    private async Task WithConnectionErrorLogging(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Connection to {@Host}:{@Port} failed", host, port);
            throw;
        }
    }

    private async Task<int> WithConnectionErrorLogging(Func<Task<int>> operation)
    {
        try
        {
            return await operation();
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Connection to {@Host}:{@Port} failed", host, port);
            throw;
        }
    }

    private TcpClient RequireOpenClient() =>
        _client ?? throw new InvalidOperationException("OpenAsync must be called before reading or writing.");
}
