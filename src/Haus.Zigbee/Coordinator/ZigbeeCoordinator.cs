using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Coordinator;

// Composes the library's protocol capabilities into the single IZigbeeCoordinator façade: it owns
// one deCONZ channel over the serial transport, connects and reads the existing network config,
// then runs a continuous background poll loop so inbound APS traffic keeps flowing for as long as
// it stays connected.
public class ZigbeeCoordinator : IZigbeeCoordinator
{
    // The dongle only surfaces inbound APS traffic when polled, so this interval trades a little
    // added latency on received reports against wasted serial bandwidth from polling too eagerly.
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);

    private readonly ISerialTransport _transport;
    private readonly DeconzChannel _channel;
    private readonly DeconzConnection _connection;
    private readonly ApsPollLoop _pollLoop;
    private readonly PermitJoinController _permitJoinController;
    private readonly ApsSender _sender;
    private readonly CommandSender _commandSender;
    private readonly AttributeReportListener _attributeReportListener;
    private readonly KnownDeviceTable _knownDeviceTable;
    private readonly DeviceInterview _deviceInterview;

    private CancellationTokenSource? _pollCancellation;
    private bool _disposed;

    public ZigbeeCoordinator(ISerialTransport transport)
    {
        _transport = transport;
        _channel = new DeconzChannel(transport);
        _connection = new DeconzConnection(_channel);
        _pollLoop = new ApsPollLoop(_channel);
        _permitJoinController = new PermitJoinController(_channel);
        _sender = new ApsSender(_pollLoop, _channel);
        _commandSender = new CommandSender(_sender);
        _attributeReportListener = new AttributeReportListener(_pollLoop);
        _knownDeviceTable = new KnownDeviceTable();
        _deviceInterview = new DeviceInterview(_pollLoop, _sender, _knownDeviceTable);

        _attributeReportListener.AttributeReported += RelayAttributeReport;
        _deviceInterview.DeviceJoined += RelayDeviceJoined;
    }

    public bool IsConnected { get; private set; }

    public NetworkConfig? NetworkConfig { get; private set; }

    public event EventHandler<ZigbeeAttributeReport>? AttributeReported;

    public event EventHandler<ZigbeeDeviceJoined>? DeviceJoined;

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

    public async Task<ZigbeeDeviceInfo?> ReadDeviceInfoAsync(IeeeAddress ieeeAddress, CancellationToken token)
    {
        if (!_knownDeviceTable.TryGet(ieeeAddress, out var device))
            return null;

        return await _deviceInterview.ReadBasicInfoAsync(device.NetworkAddress, device.Endpoints, token);
    }

    public Task SetPermitJoinAsync(bool enabled, CancellationToken token)
    {
        return _permitJoinController.SetPermitJoinAsync(enabled, token);
    }

    public Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token)
    {
        return _commandSender.SendCommandAsync(request, token);
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        StopPolling();
        _deviceInterview.DeviceJoined -= RelayDeviceJoined;
        _attributeReportListener.AttributeReported -= RelayAttributeReport;
        _deviceInterview.Dispose();
        _attributeReportListener.Dispose();
        _sender.Dispose();
        _transport.Dispose();
        GC.SuppressFinalize(this);
    }

    private void RelayAttributeReport(object? sender, ZigbeeAttributeReport report)
    {
        AttributeReported?.Invoke(this, report);
    }

    private void RelayDeviceJoined(object? sender, ZigbeeDeviceJoined device)
    {
        DeviceJoined?.Invoke(this, device);
    }

    private void StartPolling()
    {
        _pollCancellation = new CancellationTokenSource();
        _ = PollContinuouslyAsync(_pollCancellation.Token);
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
