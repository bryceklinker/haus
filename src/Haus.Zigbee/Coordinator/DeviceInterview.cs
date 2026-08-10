using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;
using Haus.Zigbee.Zdp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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

    private static readonly TimeSpan DefaultResponseTimeout = TimeSpan.FromSeconds(30);

    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;
    private readonly KnownDeviceTable _knownDeviceTable;
    private readonly TimeSpan _responseTimeout;
    private readonly ILogger<DeviceInterview> _logger;
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
        TimeSpan? responseTimeout = null,
        ILogger<DeviceInterview>? logger = null
    )
    {
        _pollLoop = pollLoop;
        _sender = sender;
        _knownDeviceTable = knownDeviceTable;
        _responseTimeout = responseTimeout ?? DefaultResponseTimeout;
        _logger = logger ?? NullLogger<DeviceInterview>.Instance;
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
        return ActiveEndpointsResponseCodec.Decode(response.AsduPayload)?.EndpointIds ?? [];
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
        var descriptor = SimpleDescriptorCodec.DecodeResponse(response.AsduPayload)?.Descriptor;
        return new ZigbeeEndpoint(
            endpointId,
            descriptor?.ProfileId ?? 0,
            descriptor?.DeviceId ?? 0,
            descriptor?.InClusters ?? [],
            descriptor?.OutClusters ?? []
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
        if (sequenceNumber is null)
        {
            _logger.LogWarning("Received a response too short to carry a transaction sequence number");
            return;
        }

        var key = new ResponseKey(indication.SourceNwkAddress, indication.ClusterId, sequenceNumber.Value);
        if (_pendingResponses.TryRemove(key, out var pending))
            pending.SetResult(indication);
        else
            _logger.LogWarning("Received a response with no matching pending request: {@Key}", key);
    }

    // ZDP responses carry their transaction sequence number as the first ASDU byte; ZCL responses
    // carry theirs inside the ZCL frame header (after the optional manufacturer code), which is why
    // this dispatches on profile rather than reading a fixed offset. Both branches can be handed a
    // malformed/truncated payload from a real device, so both return null instead of throwing.
    private static byte? ExtractTransactionSequenceNumber(ApsDataIndicationFrame indication)
    {
        if (indication.ProfileId == ZdpProfileId)
            return indication.AsduPayload.Length > 0 ? indication.AsduPayload[0] : null;

        return ZclFrameHeaderCodec.Decode(indication.AsduPayload)?.Header.TransactionSequenceNumber;
    }

    private static ZigbeeDeviceInfo ReadBasicInfo(byte[] frame)
    {
        var decoding = ZclFrameHeaderCodec.Decode(frame);
        if (decoding is null || decoding.Header.CommandId != ReadAttributesResponseCommandId)
            return ZigbeeDeviceInfo.Empty;

        var result = ZclAttributeReportParser.ParseReadAttributesResponse(frame.AsSpan(decoding.ByteLength));
        return new ZigbeeDeviceInfo(
            FindStringAttribute(result.Attributes, ManufacturerNameAttribute),
            FindStringAttribute(result.Attributes, ModelIdentifierAttribute)
        );
    }

    private static string FindStringAttribute(IReadOnlyList<ZclReadAttributeRecord> attributes, ushort attributeId)
    {
        return attributes.FirstOrDefault(attribute => attribute.AttributeId == attributeId)?.Value?.AsString()
            ?? string.Empty;
    }

    private static bool IsDeviceAnnounce(ApsDataIndicationFrame indication)
    {
        return indication.ProfileId == ZdpProfileId && indication.ClusterId == DeviceAnnounceCluster;
    }

    // Interviews run as detached tasks off the indication event; logging the outcome here (and
    // touching faulted.Exception at all) keeps a faulted interview from also surfacing later as an
    // unobserved-task exception. A timed-out SendAsync ends the task Canceled, not Faulted, so both
    // need handling -- OnlyOnFaulted alone silently drops every timeout.
    private void Forget(Task task)
    {
        task.ContinueWith(
            completed =>
            {
                if (completed.IsFaulted)
                    _logger.LogError(completed.Exception, "Detached device-interview task faulted");
                else if (completed.IsCanceled)
                    _logger.LogWarning("Detached device-interview task timed out or was canceled");
            },
            CancellationToken.None,
            TaskContinuationOptions.NotOnRanToCompletion,
            TaskScheduler.Default
        );
    }

    private readonly record struct ResponseKey(ushort NetworkAddress, ushort ClusterId, byte SequenceNumber);
}
