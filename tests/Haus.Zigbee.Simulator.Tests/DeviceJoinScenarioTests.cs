using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Zigbee.Simulator.Tests;

public class DeviceJoinScenarioTests : IAsyncLifetime
{
    private TcpListener? _listener;
    private int _port;
    private CancellationTokenSource? _acceptLoopCancellation;
    private DeconzResponder? _responder;

    public Task InitializeAsync()
    {
        _responder = new DeconzResponder();
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        _port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _acceptLoopCancellation = new CancellationTokenSource();
        _ = AcceptLoopAsync(_acceptLoopCancellation.Token);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _acceptLoopCancellation?.Cancel();
        _listener?.Stop();
        return Task.CompletedTask;
    }

    private async Task AcceptLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var client = await _listener!.AcceptTcpClientAsync(token);
                _ = new DongleConnection(client, _responder!, NullLogger<DongleConnection>.Instance).RunAsync(token);
            }
        }
        catch (OperationCanceledException) { }
    }

    [Fact]
    public async Task SimulatedDeviceJoin_RaisesDeviceJoinedWithTheRequestedIdentity()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        using var coordinator = new ZigbeeCoordinator(transport);
        var joined = new TaskCompletionSource<ZigbeeDeviceJoined>();
        coordinator.DeviceJoined += (_, device) => joined.TrySetResult(device);
        await coordinator.ConnectAsync(CancellationToken.None);

        var address = new IeeeAddress(0x00124b0001aabbcc);
        DeviceJoinScenario.SimulateJoin(_responder!, address, networkAddress: 0x1a2b, "Philips", "929002335001");

        var device = await WaitFor(joined.Task);
        Assert.Equal(address, device.IeeeAddress);
        Assert.Equal((ushort)0x1a2b, device.NetworkAddress);
        Assert.Equal("Philips", device.ManufacturerName);
        Assert.Equal("929002335001", device.ModelIdentifier);
        var endpoint = Assert.Single(device.Endpoints);
        Assert.Equal(0x01, endpoint.EndpointId);
    }

    private static async Task<T> WaitFor<T>(Task<T> task)
    {
        var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.Same(task, completed);
        return await task;
    }
}
