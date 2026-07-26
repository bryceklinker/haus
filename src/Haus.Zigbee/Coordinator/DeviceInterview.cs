using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zdp;

namespace Haus.Zigbee.Coordinator;

// Runs the standard Zigbee device-interview when a device announces itself: discover its active
// endpoints, describe each endpoint, read the Basic-cluster manufacturer/model, then raise a
// single protocol-level device-joined event with the raw result. Response indications for each
// step arrive asynchronously through the shared poll loop, so each outstanding step registers a
// completion source keyed by (source network address, expected response cluster) and awaits it.
public sealed class DeviceInterview
{
    private const ushort ZdpProfileId = 0x0000;
    private const ushort DeviceAnnounceCluster = 0x0013;
    private const ushort ActiveEndpointsRequestCluster = 0x0005;
    private const ushort ActiveEndpointsResponseCluster = 0x8005;
    private const byte ZdpEndpoint = 0x00;

    private readonly ApsSender _sender;
    private readonly KnownDeviceTable _knownDeviceTable;
    private readonly ConcurrentDictionary<ResponseKey, TaskCompletionSource<ApsDataIndicationFrame>> _pendingResponses =
        new();
    private byte _zdpTransactionSequenceNumber;
    private byte _requestId;

    public DeviceInterview(ApsPollLoop pollLoop, ApsSender sender, KnownDeviceTable knownDeviceTable)
    {
        _sender = sender;
        _knownDeviceTable = knownDeviceTable;
        pollLoop.IndicationReceived += OnIndicationReceived;
    }

    public event EventHandler<ZigbeeDeviceJoined>? DeviceJoined;

    private void OnIndicationReceived(object? sender, ApsIndicationReceived received)
    {
        var indication = received.Indication;
        if (IsDeviceAnnounce(indication))
        {
            _ = InterviewAsync(DeviceAnnounceParser.Parse(indication.AsduPayload), CancellationToken.None);
            return;
        }

        CompletePendingResponse(indication);
    }

    private async Task InterviewAsync(DeviceAnnounce announce, CancellationToken token)
    {
        var endpoints = await DiscoverEndpointsAsync(announce.NetworkAddress, token);

        var joined = new ZigbeeDeviceJoined(
            announce.IeeeAddress,
            announce.NetworkAddress,
            endpoints,
            ManufacturerName: string.Empty,
            ModelIdentifier: string.Empty
        );
        _knownDeviceTable.AddOrUpdate(new ZigbeeDevice(announce.IeeeAddress, announce.NetworkAddress, endpoints));
        DeviceJoined?.Invoke(this, joined);
    }

    private async Task<IReadOnlyList<ZigbeeEndpoint>> DiscoverEndpointsAsync(
        ushort networkAddress,
        CancellationToken token
    )
    {
        var request = ActiveEndpointsRequestCodec.Encode(
            new ActiveEndpointsRequest(_zdpTransactionSequenceNumber++, networkAddress)
        );
        await SendZdpAsync(networkAddress, ActiveEndpointsRequestCluster, ActiveEndpointsResponseCluster, request, token);
        return new List<ZigbeeEndpoint>();
    }

    private Task<ApsDataIndicationFrame> SendZdpAsync(
        ushort networkAddress,
        ushort requestCluster,
        ushort responseCluster,
        byte[] asdu,
        CancellationToken token
    )
    {
        var pending = RegisterPending(networkAddress, responseCluster);
        var request = new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: _requestId++,
            Destination: ApsDestination.Nwk(networkAddress, ZdpEndpoint),
            ProfileId: ZdpProfileId,
            ClusterId: requestCluster,
            SourceEndpoint: ZdpEndpoint,
            AsduPayload: asdu,
            TxOptions: 0x00,
            Radius: 0x00
        );
        Forget(_sender.SendAsync(request, token));
        return pending.Task;
    }

    private TaskCompletionSource<ApsDataIndicationFrame> RegisterPending(ushort networkAddress, ushort responseCluster)
    {
        var pending = new TaskCompletionSource<ApsDataIndicationFrame>();
        _pendingResponses[new ResponseKey(networkAddress, responseCluster)] = pending;
        return pending;
    }

    private void CompletePendingResponse(ApsDataIndicationFrame indication)
    {
        var key = new ResponseKey(indication.SourceNwkAddress, indication.ClusterId);
        if (_pendingResponses.TryRemove(key, out var pending))
            pending.SetResult(indication);
    }

    private static bool IsDeviceAnnounce(ApsDataIndicationFrame indication)
    {
        return indication.ProfileId == ZdpProfileId && indication.ClusterId == DeviceAnnounceCluster;
    }

    private static void Forget(Task task)
    {
        task.ContinueWith(
            faulted => _ = faulted.Exception,
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default
        );
    }

    private readonly record struct ResponseKey(ushort NetworkAddress, ushort ClusterId);
}
