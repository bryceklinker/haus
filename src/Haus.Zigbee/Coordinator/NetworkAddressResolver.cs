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

// Resolves an IEEE address to its current NWK (short) address on demand by broadcasting a ZDP
// NWK_addr_req and waiting for the one device that owns that IEEE address to answer. This is a
// narrower job than DeviceInterview's full join orchestration and has a different correlation shape:
// a broadcast request means no NWK address is known up front, so pending responses can only be keyed
// by transaction sequence number, not by (address, cluster, sequence) the way DeviceInterview does.
public class NetworkAddressResolver : IDisposable
{
    private const ushort ZdpProfileId = 0x0000;
    private const ushort NwkAddrRequestCluster = 0x0000;
    private const ushort NwkAddrResponseCluster = 0x8000;
    private const ushort BroadcastAddress = 0xffff;
    private const byte ZdpEndpoint = 0x00;
    private const byte DefaultTxOptions = 0x00;
    private const byte DefaultRadius = 0x00;

    private static readonly TimeSpan DefaultResponseTimeout = TimeSpan.FromSeconds(30);

    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;
    private readonly KnownDeviceTable _knownDeviceTable;
    private readonly TimeSpan _responseTimeout;
    private readonly ILogger<NetworkAddressResolver> _logger;
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<ApsDataIndicationFrame>> _pendingResponses = new();
    private readonly ByteSequenceCounter _transactionSequenceNumber = new();
    private readonly ByteSequenceCounter _requestId = new();

    public NetworkAddressResolver(
        ApsPollLoop pollLoop,
        ApsSender sender,
        KnownDeviceTable knownDeviceTable,
        TimeSpan? responseTimeout = null,
        ILogger<NetworkAddressResolver>? logger = null
    )
    {
        _pollLoop = pollLoop;
        _sender = sender;
        _knownDeviceTable = knownDeviceTable;
        _responseTimeout = responseTimeout ?? DefaultResponseTimeout;
        _logger = logger ?? NullLogger<NetworkAddressResolver>.Instance;
        _pollLoop.IndicationReceived += OnIndicationReceived;
    }

    public void Dispose()
    {
        _pollLoop.IndicationReceived -= OnIndicationReceived;
        GC.SuppressFinalize(this);
    }

    // Returns the resolved NWK address, or null when no device answered before the timeout. "No
    // answer" is an expected outcome the caller branches on, so a timeout resolves to null here
    // rather than throwing OperationCanceledException the way DeviceInterview/ApsSender do.
    public async Task<ushort?> ResolveAsync(IeeeAddress ieeeAddress, CancellationToken token)
    {
        var sequenceNumber = _transactionSequenceNumber.Next();
        var pending = new TaskCompletionSource<ApsDataIndicationFrame>();
        _pendingResponses[sequenceNumber] = pending;
        try
        {
            SendBroadcastRequest(ieeeAddress, sequenceNumber, token);

            using var timeout = new CancellationTokenSource(_responseTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, timeout.Token);
            var indication = await pending.Task.WaitAsync(linked.Token);
            return RecordResolvedAddress(ieeeAddress, indication);
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            return null;
        }
        finally
        {
            _pendingResponses.TryRemove(sequenceNumber, out _);
        }
    }

    private void SendBroadcastRequest(IeeeAddress ieeeAddress, byte sequenceNumber, CancellationToken token)
    {
        var asdu = NwkAddrRequestCodec.Encode(new NwkAddrRequest(sequenceNumber, ieeeAddress));
        var request = new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: _requestId.Next(),
            Destination: ApsDestination.Nwk(BroadcastAddress, ZdpEndpoint),
            ProfileId: ZdpProfileId,
            ClusterId: NwkAddrRequestCluster,
            SourceEndpoint: ZdpEndpoint,
            AsduPayload: asdu,
            TxOptions: DefaultTxOptions,
            Radius: DefaultRadius
        );
        Forget(_sender.SendAsync(request, token));
    }

    private ushort? RecordResolvedAddress(IeeeAddress ieeeAddress, ApsDataIndicationFrame indication)
    {
        var response = NwkAddrResponseCodec.Decode(indication.AsduPayload);
        if (response is null || response.Status != ZdoStatus.Success)
            return null;
        if (response.IeeeAddress != ieeeAddress)
            return null;

        var endpoints = _knownDeviceTable.TryGet(ieeeAddress, out var existing) ? existing.Endpoints : [];
        _knownDeviceTable.AddOrUpdate(new ZigbeeDevice(ieeeAddress, response.NetworkAddress, endpoints));
        return response.NetworkAddress;
    }

    // This resolver is one of several independent listeners on the shared IndicationReceived event,
    // so an indication it does not own simply belongs to someone else -- it is ignored silently
    // rather than warn-logged the way DeviceInterview's single-owner correlation does.
    private void OnIndicationReceived(object? sender, ApsIndicationReceived received)
    {
        var indication = received.Indication;
        if (indication.ProfileId != ZdpProfileId || indication.ClusterId != NwkAddrResponseCluster)
            return;
        if (indication.AsduPayload.Length == 0)
            return;

        var sequenceNumber = indication.AsduPayload[0];
        if (_pendingResponses.TryRemove(sequenceNumber, out var pending))
            pending.SetResult(indication);
    }

    // The broadcast send is not awaited here: this protocol layer treats the NWK_addr_rsp indication
    // itself as completion. Observing the detached send's outcome keeps a faulted or timed-out send
    // (ApsSender ends a confirm timeout Canceled, not Faulted) from later surfacing as an
    // unobserved-task exception.
    private void Forget(Task task)
    {
        task.ContinueWith(
            completed =>
            {
                if (completed.IsFaulted)
                    _logger.LogError(completed.Exception, "Detached NWK-address broadcast task faulted");
                else if (completed.IsCanceled)
                    _logger.LogWarning("Detached NWK-address broadcast task timed out or was canceled");
            },
            CancellationToken.None,
            TaskContinuationOptions.NotOnRanToCompletion,
            TaskScheduler.Default
        );
    }
}
