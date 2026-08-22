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

    // Guards every read AND write of _components and _transportNeedsRebuild. A plain field
    // (even one reference-swapped atomically) gives no cross-thread visibility guarantee on its
    // own -- without this lock, ConnectAsync running on one thread could observe a stale
    // "no rebuild needed" flag written by HandleTransportFailure on another thread just
    // beforehand, and go on to reconnect through the transport that call had just disposed. The
    // lock body only ever does cheap, synchronous field access/object construction, never I/O,
    // so it's never held across an await.
    private readonly object _rebuildGate = new();

    // Only ever read/written inside a `lock (_rebuildGate)` block -- see above.
    private TransportComponents _components = null!;
    private bool _transportNeedsRebuild;

    // IsConnected is polled from another thread (ZigbeeHausBridge's reconnect supervisor,
    // ZigbeeHostHealthPublisher) and needs the same cross-thread visibility guarantee; volatile
    // is enough on its own since it's a single independent flag, not part of the
    // components/rebuild compound state above.
    private volatile bool _isConnected;

    private CancellationTokenSource? _pollCancellation;
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

        // No lock needed here: the constructor runs before this instance is published to any
        // other thread, so nothing else can be reading _components concurrently yet.
        _components = BuildTransportComponents();
    }

    public ZigbeeCoordinator(ISerialTransport transport, ILoggerFactory? loggerFactory = null)
        : this(() => transport, loggerFactory) { }

    public bool IsConnected => _isConnected;

    public NetworkConfig? NetworkConfig { get; private set; }

    public event EventHandler<ZigbeeAttributeReport>? AttributeReported;

    public event EventHandler<ZigbeeDeviceJoined>? DeviceJoined;

    public async Task ConnectAsync(CancellationToken token)
    {
        var components = AcquireComponentsForConnect();
        try
        {
            await components.Transport.OpenAsync(token);
            NetworkConfig = await components.Connection.ConnectAsync(token);
        }
        catch
        {
            // A failed open/handshake can't be trusted to retry cleanly in place (e.g. our own
            // round-trip timeout already disposed this attempt's transport), so the next
            // ConnectAsync call -- not just the next poll -- must also get a fresh one instead of
            // repeatedly retrying the same broken bundle forever.
            MarkFailedForRebuild(components);
            throw;
        }

        _isConnected = true;
        StartPolling(components);
    }

    public Task<IReadOnlyList<ZigbeeDevice>> GetDevicesAsync(CancellationToken token)
    {
        return Task.FromResult(_knownDeviceTable.GetDevices());
    }

    public async Task<ZigbeeDeviceInfo?> ReadDeviceInfoAsync(IeeeAddress ieeeAddress, CancellationToken token)
    {
        if (!_knownDeviceTable.TryGet(ieeeAddress, out var device))
            return null;

        var components = CurrentComponents();
        return await components.DeviceInterview.ReadBasicInfoAsync(device.NetworkAddress, device.Endpoints, token);
    }

    public Task SetPermitJoinAsync(bool enabled, CancellationToken token)
    {
        return CurrentComponents().PermitJoinController.SetPermitJoinAsync(enabled, token);
    }

    public Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token)
    {
        return CurrentComponents().CommandSender.SendCommandAsync(request, token);
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        StopPolling();
        DisposeComponents(CurrentComponents());
        GC.SuppressFinalize(this);
    }

    private TransportComponents AcquireComponentsForConnect()
    {
        lock (_rebuildGate)
        {
            if (_transportNeedsRebuild)
            {
                _components = BuildTransportComponents();
                _transportNeedsRebuild = false;
            }
            return _components;
        }
    }

    private TransportComponents CurrentComponents()
    {
        lock (_rebuildGate)
            return _components;
    }

    // Builds a fresh, fully-wired set of every component that depends on the current transport
    // instance. Called once from the constructor and again whenever a fatal transport failure
    // marks the existing set as dead, so the next connect attempt talks to a freshly built
    // transport instead of the broken one.
    private TransportComponents BuildTransportComponents()
    {
        var transport = _transportFactory();
        var channel = new DeconzChannel(transport, _channelRoundTripTimeout);
        var connection = new DeconzConnection(channel);
        var pollLoop = new ApsPollLoop(channel);
        var permitJoinController = new PermitJoinController(channel);
        var sender = new ApsSender(pollLoop, channel);
        var commandSender = new CommandSender(sender);
        var attributeReportListener = new AttributeReportListener(pollLoop);
        var deviceInterview = new DeviceInterview(
            pollLoop,
            sender,
            _knownDeviceTable,
            logger: _loggerFactory.CreateLogger<DeviceInterview>()
        );

        attributeReportListener.AttributeReported += RelayAttributeReport;
        deviceInterview.DeviceJoined += RelayDeviceJoined;

        return new TransportComponents(
            transport,
            channel,
            connection,
            pollLoop,
            permitJoinController,
            sender,
            commandSender,
            attributeReportListener,
            deviceInterview
        );
    }

    private void DisposeComponents(TransportComponents components)
    {
        components.DeviceInterview.DeviceJoined -= RelayDeviceJoined;
        components.AttributeReportListener.AttributeReported -= RelayAttributeReport;
        components.Dispose();
    }

    private void RelayAttributeReport(object? sender, ZigbeeAttributeReport report)
    {
        AttributeReported?.Invoke(this, report);
    }

    private void RelayDeviceJoined(object? sender, ZigbeeDeviceJoined device)
    {
        DeviceJoined?.Invoke(this, device);
    }

    private void StartPolling(TransportComponents components)
    {
        _pollCancellation = new CancellationTokenSource();
        _ = PollContinuouslyAsync(components, _pollCancellation.Token);
    }

    private void StopPolling()
    {
        _pollCancellation?.Cancel();
        _pollCancellation?.Dispose();
    }

    private async Task PollContinuouslyAsync(TransportComponents components, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!await PollSafelyAsync(components.PollLoop, token))
            {
                HandleTransportFailure(components);
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
    private async Task<bool> PollSafelyAsync(ApsPollLoop pollLoop, CancellationToken token)
    {
        try
        {
            await pollLoop.PollOnceAsync(token);
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

    // Takes the exact bundle the failing poll loop was using -- rather than re-reading the
    // _components field -- because by the time this runs, a concurrent ConnectAsync could
    // already have rebuilt (e.g. this failure was detected slightly late), and disposing
    // whatever _components happens to hold *then* would tear down a perfectly healthy, currently
    // in-use bundle instead of the one that actually failed.
    //
    // IsConnected is flipped false only once this bundle is fully torn down and
    // _transportNeedsRebuild is set, not before -- ZigbeeHausBridge's reconnect supervisor reacts
    // to IsConnected becoming false, so if it observed that flip any earlier it could call
    // ConnectAsync while cleanup was still in flight on another thread.
    private void HandleTransportFailure(TransportComponents failedComponents)
    {
        MarkFailedForRebuild(failedComponents);
        _pollCancellation?.Dispose();
        _pollCancellation = null;
        _isConnected = false;
    }

    private void MarkFailedForRebuild(TransportComponents failedComponents)
    {
        lock (_rebuildGate)
        {
            _transportNeedsRebuild = true;
        }

        DisposeComponents(failedComponents);
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
