using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Haus.Zigbee.Transport;

// The real seam adapter over System.IO.Ports.SerialPort. It has no way to be unit-tested
// without physical hardware, so it deliberately carries no logic beyond delegating to the
// underlying port; every layer above it is exercised through ISerialTransport doubles instead.
public class SerialPortTransport(string portName, int baudRate, ILogger<SerialPortTransport>? logger = null)
    : ISerialTransport
{
    private readonly SerialPort _serialPort = new(portName, baudRate);
    private readonly ILogger<SerialPortTransport> _logger = logger ?? NullLogger<SerialPortTransport>.Instance;

    public Task OpenAsync(CancellationToken token)
    {
        _serialPort.Open();
        _logger.LogInformation("Opened serial port {@PortName} at {@BaudRate} baud", portName, baudRate);
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken token)
    {
        _serialPort.Close();
        _logger.LogInformation("Closed serial port {@PortName}", portName);
        return Task.CompletedTask;
    }

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) =>
        WithConnectionErrorLogging(() => _serialPort.BaseStream.WriteAsync(buffer, token).AsTask());

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token) =>
        WithConnectionErrorLogging(() => _serialPort.BaseStream.ReadAsync(buffer, token).AsTask());

    public void Dispose() => _serialPort.Dispose();

    private async Task WithConnectionErrorLogging(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Serial port {@PortName} failed", portName);
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
            _logger.LogWarning(ex, "Serial port {@PortName} failed", portName);
            throw;
        }
    }
}
