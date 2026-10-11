using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zdp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Haus.Zigbee.Coordinator;

// Queries a known device's rx-on-when-idle capability bit on demand via ZDP Node Descriptor
// Request/Response (0x0002/0x8002) -- the path to that bit when a live Device_annce hasn't
// re-fired (e.g. after a host restart), rather than relying solely on join-time announce capture.
public class NodeDescriptorQuery : IDisposable
{
    private const ushort ZdpProfileId = 0x0000;
    private const ushort NodeDescriptorRequestCluster = 0x0002;
    private const ushort NodeDescriptorResponseCluster = 0x8002;
    private const byte ZdpEndpoint = 0x00;
    private const byte DefaultTxOptions = 0x00;
    private const byte DefaultRadius = 0x00;

    private static readonly TimeSpan DefaultResponseTimeout = TimeSpan.FromSeconds(30);

    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;
    private readonly KnownDeviceTable _knownDeviceTable;
    private readonly TimeSpan _responseTimeout;
    private readonly ILogger<NodeDescriptorQuery> _logger;
    private readonly ConcurrentDictionary<ResponseKey, TaskCompletionSource<ApsDataIndicationFrame>> _pendingResponses =
        new();
    private readonly ByteSequenceCounter _transactionSequenceNumber = new();
    private readonly ByteSequenceCounter _requestId = new();

    public NodeDescriptorQuery(
        ApsPollLoop pollLoop,
        ApsSender sender,
        KnownDeviceTable knownDeviceTable,
        TimeSpan? responseTimeout = null,
        ILogger<NodeDescriptorQuery>? logger = null
    )
    {
        _pollLoop = pollLoop;
        _sender = sender;
        _knownDeviceTable = knownDeviceTable;
        _responseTimeout = responseTimeout ?? DefaultResponseTimeout;
        _logger = logger ?? NullLogger<NodeDescriptorQuery>.Instance;
        _pollLoop.IndicationReceived += OnIndicationReceived;
    }

    public void Dispose()
    {
        _pollLoop.IndicationReceived -= OnIndicationReceived;
        GC.SuppressFinalize(this);
    }

    // Returns null when the device is unknown, the request times out, or the response reports a
    // non-Success status; otherwise refreshes KnownDeviceTable's persisted sleepy state from the
    // freshly-queried value and returns it.
    public async Task<bool?> QueryIsSleepyAsync(IeeeAddress ieeeAddress, CancellationToken token)
    {
        if (!_knownDeviceTable.TryGet(ieeeAddress, out var device))
            return null;

        var sequenceNumber = _transactionSequenceNumber.Next();
        var key = new ResponseKey(device.NetworkAddress, sequenceNumber);
        var pending = new TaskCompletionSource<ApsDataIndicationFrame>();
        _pendingResponses[key] = pending;
        try
        {
            SendRequest(device.NetworkAddress, sequenceNumber, token);

            using var timeout = new CancellationTokenSource(_responseTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, timeout.Token);
            var indication = await pending.Task.WaitAsync(linked.Token);

            var response = NodeDescriptorResponseCodec.Decode(indication.AsduPayload);
            if (response is null || response.Status != ZdoStatus.Success)
                return null;

            _knownDeviceTable.UpdateIsSleepy(ieeeAddress, response.IsSleepy);
            return response.IsSleepy;
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            return null;
        }
        finally
        {
            _pendingResponses.TryRemove(key, out _);
        }
    }

    private void SendRequest(ushort networkAddress, byte sequenceNumber, CancellationToken token)
    {
        var asdu = NodeDescriptorRequestCodec.Encode(new NodeDescriptorRequest(sequenceNumber, networkAddress));
        var request = new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: _requestId.Next(),
            Destination: ApsDestination.Nwk(networkAddress, ZdpEndpoint),
            ProfileId: ZdpProfileId,
            ClusterId: NodeDescriptorRequestCluster,
            SourceEndpoint: ZdpEndpoint,
            AsduPayload: asdu,
            TxOptions: DefaultTxOptions,
            Radius: DefaultRadius
        );
        Forget(_sender.SendAsync(request, token));
    }

    // One of several independent listeners on the shared IndicationReceived event -- an indication
    // it does not own simply belongs to someone else and is ignored silently.
    private void OnIndicationReceived(object? sender, ApsIndicationReceived received)
    {
        var indication = received.Indication;
        if (indication.ProfileId != ZdpProfileId || indication.ClusterId != NodeDescriptorResponseCluster)
            return;
        if (indication.AsduPayload.Length == 0)
            return;

        var key = new ResponseKey(indication.SourceNwkAddress, indication.AsduPayload[0]);
        if (_pendingResponses.TryRemove(key, out var pending))
            pending.SetResult(indication);
    }

    // The request send is not awaited here: this protocol layer treats the Node Descriptor
    // response indication itself as completion. Observing the detached send's outcome keeps a
    // faulted or timed-out send from later surfacing as an unobserved-task exception.
    private void Forget(Task task)
    {
        task.ContinueWith(
            completed =>
            {
                if (completed.IsFaulted)
                    _logger.LogError(completed.Exception, "Detached Node Descriptor request task faulted");
                else if (completed.IsCanceled)
                    _logger.LogWarning("Detached Node Descriptor request task timed out or was canceled");
            },
            CancellationToken.None,
            TaskContinuationOptions.NotOnRanToCompletion,
            TaskScheduler.Default
        );
    }

    private readonly record struct ResponseKey(ushort NetworkAddress, byte SequenceNumber);
}
