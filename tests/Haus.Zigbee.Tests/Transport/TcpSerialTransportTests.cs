using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Transport;
using Xunit;

namespace Haus.Zigbee.Tests.Transport;

public class TcpSerialTransportTests : IAsyncLifetime
{
    private TcpListener? _listener;
    private int _port;

    public Task InitializeAsync()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        _port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _listener?.Stop();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task OpenAsync_ConnectsToTheConfiguredHostAndPort()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        var acceptTask = _listener!.AcceptTcpClientAsync();

        await transport.OpenAsync(CancellationToken.None);

        using var accepted = await acceptTask;
        Assert.True(accepted.Connected);
    }

    [Fact]
    public async Task WriteAsync_SendsBytesToTheConnectedPeer()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        var acceptTask = _listener!.AcceptTcpClientAsync();
        await transport.OpenAsync(CancellationToken.None);
        using var accepted = await acceptTask;

        await transport.WriteAsync(new byte[] { 0x01, 0x02, 0x03 }, CancellationToken.None);

        var buffer = new byte[3];
        var read = await accepted.GetStream().ReadAsync(buffer, CancellationToken.None);
        Assert.Equal(3, read);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, buffer);
    }

    [Fact]
    public async Task ReadAsync_ReceivesBytesFromTheConnectedPeer()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        var acceptTask = _listener!.AcceptTcpClientAsync();
        await transport.OpenAsync(CancellationToken.None);
        using var accepted = await acceptTask;
        await accepted.GetStream().WriteAsync(new byte[] { 0xaa, 0xbb }, CancellationToken.None);

        var buffer = new byte[2];
        var read = await transport.ReadAsync(buffer, CancellationToken.None);

        Assert.Equal(2, read);
        Assert.Equal(new byte[] { 0xaa, 0xbb }, buffer);
    }

    [Fact]
    public async Task WriteAsync_WhenCalledBeforeOpenAsyncThenThrowsInvalidOperationInsteadOfNullReference()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => transport.WriteAsync(new byte[] { 0x01 }, CancellationToken.None)
        );
    }

    [Fact]
    public async Task ReadAsync_WhenCalledBeforeOpenAsyncThenThrowsInvalidOperationInsteadOfNullReference()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);

        await Assert.ThrowsAsync<InvalidOperationException>(() => transport.ReadAsync(new byte[1], CancellationToken.None));
    }

    [Fact]
    public async Task CloseAsync_ClosesTheConnection()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        var acceptTask = _listener!.AcceptTcpClientAsync();
        await transport.OpenAsync(CancellationToken.None);
        using var accepted = await acceptTask;

        await transport.CloseAsync(CancellationToken.None);

        var buffer = new byte[1];
        var read = await accepted.GetStream().ReadAsync(buffer, CancellationToken.None);
        Assert.Equal(0, read);
    }
}
