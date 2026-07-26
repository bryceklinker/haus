using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Coordinator;

// Composes the library's protocol capabilities into the single IZigbeeCoordinator façade: it owns
// one deCONZ channel over the serial transport, connects and reads the existing network config,
// then runs a continuous background poll loop so inbound APS traffic keeps flowing for as long as
// it stays connected.
public class ZigbeeCoordinator : IZigbeeCoordinator, IDisposable
{
    // The dongle only surfaces inbound APS traffic when polled, so this interval trades a little
    // added latency on received reports against wasted serial bandwidth from polling too eagerly.
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);

    private readonly ISerialTransport _transport;
    private readonly DeconzChannel _channel;
    private readonly DeconzConnection _connection;
    private readonly ApsPollLoop _pollLoop;
    private readonly KnownDeviceTable _knownDeviceTable;

    private CancellationTokenSource? _pollCancellation;
    private Task _pollTask = Task.CompletedTask;

    public ZigbeeCoordinator(ISerialTransport transport)
    {
        _transport = transport;
        _channel = new DeconzChannel(transport);
        _connection = new DeconzConnection(_channel);
        _pollLoop = new ApsPollLoop(_channel);
        _knownDeviceTable = new KnownDeviceTable();
    }

    public bool IsConnected { get; private set; }

    public NetworkConfig? NetworkConfig { get; private set; }

    public async Task ConnectAsync(CancellationToken token)
    {
        await _transport.OpenAsync(token);
        NetworkConfig = await _connection.ConnectAsync(token);
        IsConnected = true;
        StartPolling();
    }

    public Task<IReadOnlyList<ZigbeeDevice>> GetDevicesAsync(CancellationToken token)
    {
        return Task.FromResult(_knownDeviceTable.GetDevices());
    }

    public void Dispose()
    {
        StopPolling();
        (_transport as IDisposable)?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void StartPolling()
    {
        _pollCancellation = new CancellationTokenSource();
        _pollTask = PollContinuouslyAsync(_pollCancellation.Token);
    }

    private void StopPolling()
    {
        _pollCancellation?.Cancel();
        _pollCancellation?.Dispose();
    }

    private async Task PollContinuouslyAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await PollSafelyAsync(token);
            await DelayBetweenPollsAsync(token);
        }
    }

    // A single failed poll must not tear down the long-running loop, so every iteration's failure
    // is swallowed and the next poll simply retries.
    private async Task PollSafelyAsync(CancellationToken token)
    {
        try
        {
            await _pollLoop.PollOnceAsync(token);
        }
        catch (Exception) { }
    }

    private static async Task DelayBetweenPollsAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(PollInterval, token);
        }
        catch (OperationCanceledException) { }
    }
}
