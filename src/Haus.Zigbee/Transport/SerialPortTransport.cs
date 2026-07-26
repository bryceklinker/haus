using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Haus.Zigbee.Transport;

// The real seam adapter over System.IO.Ports.SerialPort. It has no way to be unit-tested
// without physical hardware, so it deliberately carries no logic beyond delegating to the
// underlying port; every layer above it is exercised through ISerialTransport doubles instead.
public class SerialPortTransport : ISerialTransport, IDisposable
{
    private readonly SerialPort _serialPort;

    public SerialPortTransport(string portName, int baudRate)
    {
        _serialPort = new SerialPort(portName, baudRate);
    }

    public Task OpenAsync(CancellationToken token)
    {
        _serialPort.Open();
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken token)
    {
        _serialPort.Close();
        return Task.CompletedTask;
    }

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) =>
        _serialPort.BaseStream.WriteAsync(buffer, token).AsTask();

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token) =>
        _serialPort.BaseStream.ReadAsync(buffer, token).AsTask();

    public void Dispose() => _serialPort.Dispose();
}
