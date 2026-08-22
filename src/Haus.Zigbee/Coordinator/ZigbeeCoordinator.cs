using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Haus.Zigbee.Coordinator;

public class ZigbeeCoordinator : IZigbeeCoordinator
{
    // The dongle only surfaces inbound APS traffic when polled, so this interval trades a little
    // added latency on received reports against wasted serial bandwidth from polling too eagerly.
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);

    private readonly Func<ISerialTransport> _transportFactory;
    private readonly TimeSpan? _channelRoundTripTimeout;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ZigbeeCoordinator> _logger;
    private readonly KnownDeviceTable _knownDeviceTable = new();

    private ISerialTransport _transport = null!;
    private DeconzChannel _channel = null!;
    private DeconzConnection _connection = null!;
    private ApsPollLoop _pollLoop = null!;
    private PermitJoinController _permitJoinController = null!;
    private ApsSender _sender = null!;
    private CommandSender _commandSender = null!;
    private AttributeReportListener _attributeReportListener = null!;
    private DeviceInterview _deviceInterview = null!;

    private CancellationTokenSource? _pollCancellation;
    private bool _transportNeedsRebuild;
    private bool _disposed;

    public ZigbeeCoordinator(
        Func<ISerialTransport> transportFactory,
        ILoggerFactory? loggerFactory = null,
        TimeSpan? channelRoundTripTimeout = null
    )
    {
        _transportFactory = transportFactory;
        _channelRoundTripTimeout = channelRoundTripTimeout;
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<ZigbeeCoordinator>();

        BuildTransportComponents();
    }

    public ZigbeeCoordinator(ISerialTransport transport, ILoggerFactory? loggerFactory = null)
        : this(() => transport, loggerFactory) { }

    public bool IsConnected { get; private set; }

    public NetworkConfig? NetworkConfig { get; private set; }

    public event EventHandler<ZigbeeAttributeReport>? AttributeReported;

    public event EventHandler<ZigbeeDeviceJoined>? DeviceJoined;

    public async Task ConnectAsync(CancellationToken token)
    {
        if (_transportNeedsRebuild)
        {
            BuildTransportComponents();
            _transportNeedsRebuild = false;
        }

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
        DisposeTransportComponents();
        GC.SuppressFinalize(this);
    }

    // (Re)wires every component that depends on the current transport instance. Called once from
    // the constructor and again whenever a fatal transport failure marks the existing set as dead,
    // so the next connect attempt talks to a freshly built transport instead of the broken one.
    private void BuildTransportComponents()
    {
        _transport = _transportFactory();
        _channel = new DeconzChannel(_transport, _channelRoundTripTimeout);
        _connection = new DeconzConnection(_channel);
        _pollLoop = new ApsPollLoop(_channel);
        _permitJoinController = new PermitJoinController(_channel);
        _sender = new ApsSender(_pollLoop, _channel);
        _commandSender = new CommandSender(_sender);
        _attributeReportListener = new AttributeReportListener(_pollLoop);
        _deviceInterview = new DeviceInterview(
            _pollLoop,
            _sender,
            _knownDeviceTable,
            logger: _loggerFactory.CreateLogger<DeviceInterview>()
        );

        _attributeReportListener.AttributeReported += RelayAttributeReport;
        _deviceInterview.DeviceJoined += RelayDeviceJoined;
    }

    private void DisposeTransportComponents()
    {
        _deviceInterview.DeviceJoined -= RelayDeviceJoined;
        _attributeReportListener.AttributeReported -= RelayAttributeReport;
        _deviceInterview.Dispose();
        _attributeReportListener.Dispose();
        _sender.Dispose();
        _transport.Dispose();
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
            if (!await PollSafelyAsync(token))
            {
                HandleTransportFailure();
                return;
            }

            await DelayBetweenPollsAsync(token);
        }
    }

    // A single transient failure must not tear down the long-running loop -- the next poll simply
    // retries. But a failure that means the transport itself is now dead (our own bounded-timeout
    // dispose, or the ObjectDisposedException a real torn-down SerialPort raises on the next call
    // against it) can never self-heal by retrying in place, so that case is reported back as fatal
    // instead of being swallowed like every other poll error.
    private async Task<bool> PollSafelyAsync(CancellationToken token)
    {
        try
        {
            await _pollLoop.PollOnceAsync(token);
            return true;
        }
        catch (OperationCanceledException)
        {
            return true;
        }
        catch (Exception ex) when (ex is SerialTransportTimeoutException or ObjectDisposedException)
        {
            _logger.LogWarning(ex, "Zigbee transport failed; marking the connection down so it can reconnect");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Poll iteration failed, will retry on the next interval");
            return true;
        }
    }

    private void HandleTransportFailure()
    {
        IsConnected = false;
        _transportNeedsRebuild = true;
        DisposeTransportComponents();
        _pollCancellation?.Dispose();
        _pollCancellation = null;
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
