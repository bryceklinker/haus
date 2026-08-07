using System;
using System.Collections.Generic;
using System.Linq;
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

    [Fact]
    public async Task SimulateJoin_CalledForTwoDevicesBeforeEitherAnnounceIsProcessed_BothDevicesJoinSuccessfully()
    {
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        using var coordinator = new ZigbeeCoordinator(transport);
        var joinedDevices = new List<ZigbeeDeviceJoined>();
        var bothJoined = new TaskCompletionSource();
        coordinator.DeviceJoined += (_, device) =>
        {
            lock (joinedDevices)
            {
                joinedDevices.Add(device);
                if (joinedDevices.Count == 2)
                    bothJoined.TrySetResult();
            }
        };
        await coordinator.ConnectAsync(CancellationToken.None);

        var addressA = new IeeeAddress(0x00124b0001aabbcc);
        var addressB = new IeeeAddress(0x00124b0001aabbdd);

        // Scheduled back-to-back, before either device's own real interview has sent a single APS
        // request -- this is what two overlapping /devices/join HTTP requests do in practice.
        DeviceJoinScenario.SimulateJoin(_responder!, addressA, networkAddress: 0x1001, "Philips", "929002335001");
        DeviceJoinScenario.SimulateJoin(_responder!, addressB, networkAddress: 0x1002, "Philips", "9290012607");

        await WaitFor(bothJoined.Task);

        Assert.Contains(
            joinedDevices,
            d => d.IeeeAddress == addressA && d.ManufacturerName == "Philips" && d.ModelIdentifier == "929002335001"
        );
        Assert.Contains(
            joinedDevices,
            d => d.IeeeAddress == addressB && d.ManufacturerName == "Philips" && d.ModelIdentifier == "9290012607"
        );
    }

    [Fact]
    public async Task SimulateJoin_CalledForEightDevicesConcurrently_EveryDeviceJoinsSuccessfully()
    {
        // Regression test for a production incident: at low concurrency (2-3 overlapping joins)
        // this always passed, but real acceptance-test-level load (multiple devices announcing at
        // once) reliably hit a bug where AttributeReportListener threw trying to parse a ZDP
        // response as a ZCL frame, which -- since it shares the poll loop's IndicationReceived
        // multicast event with DeviceInterview -- silently stopped DeviceInterview's own handler
        // from ever running for that indication, permanently stalling that one device's interview.
        // Eight concurrent devices reproduced it within a handful of runs; two rarely did.
        const int deviceCount = 8;
        using var transport = new TcpSerialTransport("127.0.0.1", _port);
        using var coordinator = new ZigbeeCoordinator(transport);
        var joinedDevices = new List<ZigbeeDeviceJoined>();
        var allJoined = new TaskCompletionSource();
        coordinator.DeviceJoined += (_, device) =>
        {
            lock (joinedDevices)
            {
                joinedDevices.Add(device);
                if (joinedDevices.Count == deviceCount)
                    allJoined.TrySetResult();
            }
        };
        await coordinator.ConnectAsync(CancellationToken.None);

        var addresses = Enumerable
            .Range(1, deviceCount)
            .Select(i => new IeeeAddress(0x00124b0001aabb00ul + (ulong)i))
            .ToList();
        for (var i = 0; i < deviceCount; i++)
        {
            DeviceJoinScenario.SimulateJoin(
                _responder!,
                addresses[i],
                networkAddress: (ushort)(0x2000 + i),
                "Philips",
                "929002335001"
            );
        }

        await WaitFor(allJoined.Task, TimeSpan.FromSeconds(15));

        Assert.Equal(deviceCount, joinedDevices.Count);
        foreach (var address in addresses)
            Assert.Contains(joinedDevices, d => d.IeeeAddress == address);
    }

    private static async Task<T> WaitFor<T>(Task<T> task)
    {
        var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.Same(task, completed);
        return await task;
    }

    private static async Task WaitFor(Task task, TimeSpan? timeout = null)
    {
        var completed = await Task.WhenAny(task, Task.Delay(timeout ?? TimeSpan.FromSeconds(5)));
        Assert.Same(task, completed);
        await task;
    }
}
