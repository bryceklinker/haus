using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Zigbee.Simulator.Tests;

public class SimulatorEndToEndTests : IAsyncLifetime
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
    public async Task RealCoordinatorConnectsToTheSimulatorAndReadsDefaultNetworkConfig()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        using var coordinator = new ZigbeeCoordinator(transport);

        await coordinator.ConnectAsync(CancellationToken.None);

        Assert.True(coordinator.IsConnected);
        var config = coordinator.NetworkConfig;
        Assert.NotNull(config);
        Assert.Equal(new IeeeAddress(0x00212effff001122), config!.MacAddress);
        Assert.Equal((ushort)0x1a62, config.PanId);
        Assert.Equal((byte)0x0f, config.Channel);
    }

    [Fact]
    public async Task RealCoordinatorEnablesPermitJoinAgainstTheSimulator()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        using var coordinator = new ZigbeeCoordinator(transport);
        await coordinator.ConnectAsync(CancellationToken.None);

        await coordinator.SetPermitJoinAsync(true, CancellationToken.None);

        Assert.Empty(await coordinator.GetDevicesAsync(CancellationToken.None));
    }
}
