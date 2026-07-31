using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;
using Haus.Zigbee.Zdp;

namespace Haus.Zigbee.Coordinator;

public class DeviceInterview : IDisposable
{
    private const ushort ZdpProfileId = 0x0000;
    private const ushort DeviceAnnounceCluster = 0x0013;
    private const ushort ActiveEndpointsRequestCluster = 0x0005;
    private const ushort ActiveEndpointsResponseCluster = 0x8005;
    private const ushort SimpleDescriptorRequestCluster = 0x0004;
    private const ushort SimpleDescriptorResponseCluster = 0x8004;
    private const ushort BasicCluster = 0x0000;
    private const ushort ManufacturerNameAttribute = 0x0004;
    private const ushort ModelIdentifierAttribute = 0x0005;

    private const byte ZdpEndpoint = 0x00;
    private const byte CoordinatorSourceEndpoint = 0x01;
    private const byte DefaultTxOptions = 0x00;
    private const byte DefaultRadius = 0x00;

    private const byte ReadAttributesResponseCommandId = 0x01;
    private const byte ZclSuccessStatus = 0x00;
    private const byte CharacterStringType = 0x42;
    private const byte InvalidStringLength = 0xff;

    private static readonly TimeSpan DefaultResponseTimeout = TimeSpan.FromSeconds(30);

    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;
    private readonly KnownDeviceTable _knownDeviceTable;
    private readonly TimeSpan _responseTimeout;
    private readonly ConcurrentDictionary<ResponseKey, TaskCompletionSource<ApsDataIndicationFrame>> _pendingResponses =
        new();

    // Interviews for different devices are dispatched as independent tasks (see Forget below) and
    // can call these concurrently, so each needs an atomic counter rather than a plain field.
    private readonly ByteSequenceCounter _zdpTransactionSequenceNumber = new();
    private readonly ByteSequenceCounter _zclTransactionSequenceNumber = new();
    private readonly ByteSequenceCounter _requestId = new();

    public DeviceInterview(
        ApsPollLoop pollLoop,
        ApsSender sender,
        KnownDeviceTable knownDeviceTable,
        TimeSpan? responseTimeout = null
    )
    {
        _pollLoop = pollLoop;
        _sender = sender;
        _knownDeviceTable = knownDeviceTable;
        _responseTimeout = responseTimeout ?? DefaultResponseTimeout;
        _pollLoop.IndicationReceived += OnIndicationReceived;
    }

    public event EventHandler<ZigbeeDeviceJoined>? DeviceJoined;

    public void Dispose()
    {
        _pollLoop.IndicationReceived -= OnIndicationReceived;
        GC.SuppressFinalize(this);
    }

    private void OnIndicationReceived(object? sender, ApsIndicationReceived received)
    {
        var indication = received.Indication;
        if (IsDeviceAnnounce(indication))
        {
            Forget(InterviewAsync(DeviceAnnounceParser.Parse(indication.AsduPayload), CancellationToken.None));
            return;
        }

        CompletePendingResponse(indication);
    }

    private async Task InterviewAsync(DeviceAnnounce announce, CancellationToken token)
    {
        var endpoints = await DescribeEndpointsAsync(announce.NetworkAddress, token).ConfigureAwait(false);
        var basicInfo = await ReadBasicInfoAsync(announce.NetworkAddress, endpoints, token).ConfigureAwait(false);

        var device = new ZigbeeDevice(announce.IeeeAddress, announce.NetworkAddress, endpoints);
        _knownDeviceTable.AddOrUpdate(device);
        DeviceJoined?.Invoke(
            this,
            new ZigbeeDeviceJoined(
                announce.IeeeAddress,
                announce.NetworkAddress,
                endpoints,
                basicInfo.ManufacturerName,
                basicInfo.ModelIdentifier
            )
        );
    }

    private async Task<IReadOnlyList<ZigbeeEndpoint>> DescribeEndpointsAsync(
        ushort networkAddress,
        CancellationToken token
    )
    {
        var endpointIds = await DiscoverEndpointIdsAsync(networkAddress, token).ConfigureAwait(false);
        var endpoints = new List<ZigbeeEndpoint>();
        foreach (var endpointId in endpointIds)
            endpoints.Add(await DescribeEndpointAsync(networkAddress, endpointId, token).ConfigureAwait(false));
        return endpoints;
    }

    private async Task<IReadOnlyList<byte>> DiscoverEndpointIdsAsync(ushort networkAddress, CancellationToken token)
    {
        var sequenceNumber = _zdpTransactionSequenceNumber.Next();
        var request = ActiveEndpointsRequestCodec.Encode(new ActiveEndpointsRequest(sequenceNumber, networkAddress));
        var response = await SendZdpAsync(
                networkAddress,
                sequenceNumber,
                ActiveEndpointsRequestCluster,
                ActiveEndpointsResponseCluster,
                request,
                token
            )
            .ConfigureAwait(false);
        return ActiveEndpointsResponseCodec.Decode(response.AsduPayload).EndpointIds;
    }

    private async Task<ZigbeeEndpoint> DescribeEndpointAsync(
        ushort networkAddress,
        byte endpointId,
        CancellationToken token
    )
    {
        var sequenceNumber = _zdpTransactionSequenceNumber.Next();
        var request = SimpleDescriptorCodec.EncodeRequest(
            new SimpleDescriptorRequest(sequenceNumber, networkAddress, endpointId)
        );
        var response = await SendZdpAsync(
                networkAddress,
                sequenceNumber,
                SimpleDescriptorRequestCluster,
                SimpleDescriptorResponseCluster,
                request,
                token
            )
            .ConfigureAwait(false);
        var descriptor = SimpleDescriptorCodec.DecodeResponse(response.AsduPayload).Descriptor;
        return new ZigbeeEndpoint(
            endpointId,
            descriptor?.ProfileId ?? 0,
            descriptor?.DeviceId ?? 0,
            descriptor?.InClusters ?? Array.Empty<ushort>(),
            descriptor?.OutClusters ?? Array.Empty<ushort>()
        );
    }

    public async Task<ZigbeeDeviceInfo> ReadBasicInfoAsync(
        ushort networkAddress,
        IReadOnlyList<ZigbeeEndpoint> endpoints,
        CancellationToken token
    )
    {
        var endpoint = SelectBasicClusterEndpoint(endpoints);
        if (endpoint is null)
            return ZigbeeDeviceInfo.Empty;

        var sequenceNumber = _zclTransactionSequenceNumber.Next();
        var request = ZclReadAttributesRequestCodec.Encode(
            new ZclReadAttributesRequest(sequenceNumber, new[] { ManufacturerNameAttribute, ModelIdentifierAttribute })
        );
        var response = await SendAsync(
                networkAddress,
                endpoint.EndpointId,
                CoordinatorSourceEndpoint,
                endpoint.ProfileId,
                BasicCluster,
                BasicCluster,
                sequenceNumber,
                request,
                token
            )
            .ConfigureAwait(false);
        return ReadBasicInfo(response.AsduPayload);
    }

    // A Basic-cluster endpoint (in-cluster 0x0000) owns the manufacturer/model attributes, but some
    // devices are lenient about advertising it, so fall back to the first endpoint when none does.
    private static ZigbeeEndpoint? SelectBasicClusterEndpoint(IReadOnlyList<ZigbeeEndpoint> endpoints)
    {
        return endpoints.FirstOrDefault(endpoint => endpoint.InClusters.Contains(BasicCluster))
            ?? endpoints.FirstOrDefault();
    }

    private Task<ApsDataIndicationFrame> SendZdpAsync(
        ushort networkAddress,
        byte sequenceNumber,
        ushort requestCluster,
        ushort responseCluster,
        byte[] asdu,
        CancellationToken token
    )
    {
        return SendAsync(
            networkAddress,
            ZdpEndpoint,
            ZdpEndpoint,
            ZdpProfileId,
            requestCluster,
            responseCluster,
            sequenceNumber,
            asdu,
            token
        );
    }

    // Registers the outstanding response before writing the request so a fast reply cannot arrive
    // before this interview is waiting for it. The delivery confirm is not awaited: this protocol
    // layer treats the ZDP/ZCL response indication itself as the interview step's completion.
    // Correlation includes the request's own transaction sequence number (not just address+cluster):
    // a device can be interviewed twice concurrently (e.g. a re-announce racing a backfill read), and
    // without the sequence number the second registration would silently orphan the first's pending
    // response.
    // A device that never sends a response would otherwise hang this forever and leak its entry in
    // _pendingResponses permanently, since nothing else ever removes it.
    private async Task<ApsDataIndicationFrame> SendAsync(
        ushort networkAddress,
        byte destinationEndpoint,
        byte sourceEndpoint,
        ushort profileId,
        ushort requestCluster,
        ushort responseCluster,
        byte sequenceNumber,
        byte[] asdu,
        CancellationToken token
    )
    {
        var key = new ResponseKey(networkAddress, responseCluster, sequenceNumber);
        var pending = RegisterPending(key);
        try
        {
            var request = new ApsDataRequestFrame(
                SequenceNumber: 0,
                RequestId: _requestId.Next(),
                Destination: ApsDestination.Nwk(networkAddress, destinationEndpoint),
                ProfileId: profileId,
                ClusterId: requestCluster,
                SourceEndpoint: sourceEndpoint,
                AsduPayload: asdu,
                TxOptions: DefaultTxOptions,
                Radius: DefaultRadius
            );
            Forget(_sender.SendAsync(request, token));

            using var timeout = new CancellationTokenSource(_responseTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, timeout.Token);
            return await pending.Task.WaitAsync(linked.Token);
        }
        finally
        {
            _pendingResponses.TryRemove(key, out _);
        }
    }

    private TaskCompletionSource<ApsDataIndicationFrame> RegisterPending(ResponseKey key)
    {
        var pending = new TaskCompletionSource<ApsDataIndicationFrame>();
        _pendingResponses[key] = pending;
        return pending;
    }

    private void CompletePendingResponse(ApsDataIndicationFrame indication)
    {
        var sequenceNumber = ExtractTransactionSequenceNumber(indication);
        var key = new ResponseKey(indication.SourceNwkAddress, indication.ClusterId, sequenceNumber);
        if (_pendingResponses.TryRemove(key, out var pending))
            pending.SetResult(indication);
    }

    // ZDP responses carry their transaction sequence number as the first ASDU byte; ZCL responses
    // carry theirs inside the ZCL frame header (after the optional manufacturer code), which is why
    // this dispatches on profile rather than reading a fixed offset.
    private static byte ExtractTransactionSequenceNumber(ApsDataIndicationFrame indication)
    {
        return indication.ProfileId == ZdpProfileId
            ? indication.AsduPayload[0]
            : ZclFrameHeaderCodec.Decode(indication.AsduPayload).Header.TransactionSequenceNumber;
    }

    private static ZigbeeDeviceInfo ReadBasicInfo(byte[] frame)
    {
        var decoding = ZclFrameHeaderCodec.Decode(frame);
        if (decoding.Header.CommandId != ReadAttributesResponseCommandId)
            return ZigbeeDeviceInfo.Empty;

        var manufacturerName = string.Empty;
        var modelIdentifier = string.Empty;
        var offset = decoding.ByteLength;
        while (TryReadAttribute(frame, ref offset, out var attributeId, out var value))
        {
            if (attributeId == ManufacturerNameAttribute)
                manufacturerName = value;
            else if (attributeId == ModelIdentifierAttribute)
                modelIdentifier = value;
        }

        return new ZigbeeDeviceInfo(manufacturerName, modelIdentifier);
    }

    private static bool TryReadAttribute(byte[] frame, ref int offset, out ushort attributeId, out string value)
    {
        attributeId = 0;
        value = string.Empty;
        const int recordHeaderLength = 3;
        if (offset + recordHeaderLength > frame.Length)
            return false;

        attributeId = (ushort)(frame[offset] | (frame[offset + 1] << 8));
        var status = frame[offset + 2];
        offset += recordHeaderLength;
        if (status != ZclSuccessStatus)
            return true;

        var dataType = frame[offset++];
        if (dataType != CharacterStringType)
            return false;

        var length = frame[offset++];
        if (length == InvalidStringLength)
            return true;

        value = Encoding.ASCII.GetString(frame, offset, length);
        offset += length;
        return true;
    }

    private static bool IsDeviceAnnounce(ApsDataIndicationFrame indication)
    {
        return indication.ProfileId == ZdpProfileId && indication.ClusterId == DeviceAnnounceCluster;
    }

    // Interviews run as detached tasks off the indication event; observing the exception keeps a
    // faulted interview from surfacing later as an unobserved-task exception.
    private static void Forget(Task task)
    {
        task.ContinueWith(
            faulted => _ = faulted.Exception,
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default
        );
    }

    private readonly record struct ResponseKey(ushort NetworkAddress, ushort ClusterId, byte SequenceNumber);
}
