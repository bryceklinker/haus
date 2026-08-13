using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Simulator;

public record IndicationBody(ushort SourceNwk, byte SourceEndpoint, ushort ProfileId, ushort ClusterId, byte[] Asdu);

// Mirrors what a real coordinator reports once it has delivered an APS data-request it was asked
// to send: which request (by RequestId), to which device/endpoint, from which of the coordinator's
// own endpoints.
public record ConfirmBody(
    byte RequestId,
    ushort DestinationNetworkAddress,
    byte DestinationEndpoint,
    byte SourceEndpoint
);

// Answers deCONZ serial commands the same way a real coordinator dongle would: reads/writes of
// firmware parameters, device-state polls, read-indication drains, and APS data-request acks.
// Registered as a DI singleton and shared for the app's lifetime: it backs both the HTTP control
// API (SimulatorController, which injects parameters/indications and drives device joins) and
// whichever TCP connection(s) are currently open, and it's precisely because both sides read and
// write the same instance that an HTTP-triggered indication can reach a connected coordinator.
// That's also why its state -- parameters, indication/confirm queues, interview scripts -- must
// be shared rather than scoped to a single connection.
public class DeconzResponder
{
    private const byte ReadParameterCommand = 0x0a;
    private const byte WriteParameterCommand = 0x0b;
    private const byte DeviceStateCommand = 0x07;
    private const byte ReadIndicationCommand = 0x17;
    private const byte ReadConfirmCommand = 0x04;
    private const byte ApsDataRequestCommand = 0x12;
    private const byte ConfirmAvailable = 0x04;
    private const byte IndicationAvailable = 0x08;
    private const byte NothingAvailable = 0x00;
    private const byte SuccessStatus = 0x00;
    private const int SequenceNumberOffset = 1;
    private const int ParameterIdOffset = 7;
    private const int WriteParameterValueOffset = 8;
    private const int RequestIdOffset = 7;

    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1c;

    // WriteParameter response is a fixed 8-byte frame (command+seq+status+frameLength(2)+
    // payloadLength(2)+parameterId) when the written value is just the parameter id -- no extra
    // bytes echoed back.
    private const ushort WriteParameterResponseFrameLength = 8;
    private const ushort WriteParameterResponsePayloadLength = 1;

    // DeviceState/ReadIndication/ApsDataRequest-ack responses all carry a frameLength (and, for
    // ReadIndication, a payloadLength) field that the real client-side decoder never validates,
    // so these are left zero rather than computed.
    private const ushort UnvalidatedFrameLength = 0;
    private const ushort UnvalidatedPayloadLength = 0;

    // Indications are always addressed to the coordinator itself: network address 0x0000 on its
    // own listening endpoint 1.
    private const ushort CoordinatorNetworkAddress = 0x0000;
    private const byte CoordinatorEndpoint = 0x01;
    private const byte NoRssiLinkQuality = 0xff;

    // Offsets into an ApsDataRequest command's payload, assuming Nwk-mode addressing (0x02) --
    // the only mode ApsDataRequestFrameCodec's callers in this codebase ever send. Mirrors the
    // layout ApsDataRequestFrameCodec.Encode produces.
    private const int DestinationAddressModeOffset = 9;
    private const int DestinationNetworkAddressOffset = DestinationAddressModeOffset + 1;
    private const int DestinationEndpointOffset = DestinationNetworkAddressOffset + 2;
    private const int NwkDestinationAddressLength = 3; // short address (2 bytes) + endpoint (1 byte)
    private const int ProfileAndClusterLength = 4; // profile id (2 bytes) + cluster id (2 bytes)
    private const int SourceEndpointLength = 1;
    private const int SourceEndpointOffset =
        DestinationAddressModeOffset + 1 + NwkDestinationAddressLength + ProfileAndClusterLength;
    private const int AsduLengthFieldLength = 2;

    private readonly ConcurrentDictionary<byte, byte[]> _parameters = new();
    private readonly ConcurrentQueue<IndicationBody> _indications = new();
    private readonly ConcurrentQueue<ConfirmBody> _confirms = new();
    private readonly ConcurrentQueue<byte[]> _sentApsRequests = new();

    // Keyed by (destination device, that device's own request step) rather than a single shared
    // request counter: two devices' interviews can have their individual ZDP/ZCL steps interleave
    // on the wire (each is its own detached task on the coordinator side), so a flat global index
    // cannot tell two devices' 1st/2nd/3rd requests apart. Per-device counting sidesteps that
    // entirely -- it needs no reservation or locking beyond what ConcurrentDictionary already gives.
    private readonly ConcurrentDictionary<
        (ushort NetworkAddress, int Step),
        Func<byte[], IndicationBody>
    > _releaseOnApsRequest = new();
    private readonly ConcurrentDictionary<ushort, int> _apsRequestCountByDevice = new();
    private int _networkAddressCounter;

    public DeconzResponder()
    {
        SetParameter(MacAddressParameterId, [0x22, 0x11, 0x00, 0xff, 0xff, 0x2e, 0x21, 0x00]);
        SetParameter(PanIdParameterId, [0x62, 0x1a]);
        SetParameter(ChannelParameterId, [0x0f]);
    }

    public void SetParameter(byte parameterId, byte[] value)
    {
        _parameters[parameterId] = value;
    }

    public void EnqueueIndication(IndicationBody body)
    {
        _indications.Enqueue(body);
    }

    // Lets a caller script a device-interview response sequence: the Nth APS data-request this
    // responder sees FOR THAT SPECIFIC DEVICE (0-based) is exactly when a real device would have
    // replied, so queuing the response there keeps request/response order faithful to a real
    // interview. The factory receives that request's raw bytes so it can echo back the request's
    // own transaction sequence number -- DeviceInterview correlates responses on that number, and
    // a real device discovers it from the request rather than knowing it in advance.
    public void ReleaseAfterApsRequest(ushort networkAddress, int step, Func<byte[], IndicationBody> bodyFactory)
    {
        _releaseOnApsRequest[(networkAddress, step)] = bodyFactory;
    }

    // Assumes Nwk-mode addressing.
    public static byte[] ExtractAsduPayload(byte[] apsDataRequest)
    {
        var asduLengthOffset = SourceEndpointOffset + SourceEndpointLength;
        var asduOffset = asduLengthOffset + AsduLengthFieldLength;
        var asduLength = BinaryPrimitives.ReadUInt16LittleEndian(apsDataRequest.AsSpan(asduLengthOffset));
        return apsDataRequest[asduOffset..(asduOffset + asduLength)];
    }

    // Assumes Nwk-mode addressing.
    public static ushort ExtractDestinationNetworkAddress(byte[] apsDataRequest)
    {
        return BinaryPrimitives.ReadUInt16LittleEndian(apsDataRequest.AsSpan(DestinationNetworkAddressOffset));
    }

    public IReadOnlyList<byte[]> SentApsRequests => _sentApsRequests.ToList();

    public int GetApsRequestCountForDevice(ushort networkAddress)
    {
        return _apsRequestCountByDevice.GetValueOrDefault(networkAddress);
    }

    // Sequential rather than derived from the device's IEEE address: two devices joining in the
    // same test run must never collide on network address, and a random 16-bit derivation can.
    public ushort AllocateNetworkAddress()
    {
        return (ushort)Interlocked.Increment(ref _networkAddressCounter);
    }

    // For request logging at the connection layer, which only has the raw command byte to go on.
    public static string DescribeCommand(byte commandId)
    {
        return commandId switch
        {
            ReadParameterCommand => "ReadParameter",
            WriteParameterCommand => "WriteParameter",
            DeviceStateCommand => "DeviceState",
            ReadIndicationCommand => "ReadIndication",
            ApsDataRequestCommand => "ApsDataRequest",
            _ => $"Unknown(0x{commandId:X2})",
        };
    }

    // Unframed, unchecksummed request in -> unframed, unchecksummed response out (or empty for a
    // command this responder doesn't recognize). The connection loop owns SLIP/checksum framing.
    public byte[] HandleRequest(byte[] request)
    {
        if (request.Length <= SequenceNumberOffset)
            return [];

        var sequenceNumber = request[SequenceNumberOffset];
        return request[0] switch
        {
            ReadParameterCommand => ReadParameterResponse(sequenceNumber, request[ParameterIdOffset]),
            WriteParameterCommand => WriteParameterResponse(request),
            DeviceStateCommand => DeviceStateResponse(sequenceNumber),
            ReadIndicationCommand => IndicationResponse(sequenceNumber),
            ReadConfirmCommand => ConfirmResponse(sequenceNumber),
            ApsDataRequestCommand => ApsDataRequestResponse(request),
            _ => [],
        };
    }

    private byte[] ReadParameterResponse(byte sequenceNumber, byte parameterId)
    {
        var value = _parameters.TryGetValue(parameterId, out var stored) ? stored : [];
        return ReadParameterFrame.Encode(new ReadParameterRequest(sequenceNumber, parameterId, value));
    }

    private byte[] WriteParameterResponse(byte[] request)
    {
        var parameterId = request[ParameterIdOffset];
        SetParameter(parameterId, request[WriteParameterValueOffset..]);
        return
        [
            WriteParameterCommand,
            request[SequenceNumberOffset],
            SuccessStatus,
            .. LittleEndian.Bytes(WriteParameterResponseFrameLength),
            .. LittleEndian.Bytes(WriteParameterResponsePayloadLength),
            parameterId,
        ];
    }

    private byte[] DeviceStateResponse(byte sequenceNumber)
    {
        byte deviceState = NothingAvailable;
        if (!_indications.IsEmpty)
            deviceState |= IndicationAvailable;
        if (!_confirms.IsEmpty)
            deviceState |= ConfirmAvailable;

        return
        [
            DeviceStateCommand,
            sequenceNumber,
            SuccessStatus,
            .. LittleEndian.Bytes(UnvalidatedFrameLength),
            deviceState,
        ];
    }

    private byte[] IndicationResponse(byte sequenceNumber)
    {
        return _indications.TryDequeue(out var body) ? EncodeIndication(sequenceNumber, body) : [];
    }

    private byte[] ConfirmResponse(byte sequenceNumber)
    {
        return _confirms.TryDequeue(out var body) ? EncodeConfirm(sequenceNumber, body) : [];
    }

    private byte[] ApsDataRequestResponse(byte[] request)
    {
        _sentApsRequests.Enqueue(request);
        var networkAddress = ExtractDestinationNetworkAddress(request);
        _confirms.Enqueue(
            new ConfirmBody(
                request[RequestIdOffset],
                networkAddress,
                request[DestinationEndpointOffset],
                request[SourceEndpointOffset]
            )
        );
        var countForDevice = _apsRequestCountByDevice.AddOrUpdate(networkAddress, 1, (_, count) => count + 1);
        var step = countForDevice - 1;
        if (_releaseOnApsRequest.TryRemove((networkAddress, step), out var bodyFactory))
            _indications.Enqueue(bodyFactory(request));

        return
        [
            ApsDataRequestCommand,
            request[SequenceNumberOffset],
            SuccessStatus,
            .. LittleEndian.Bytes(UnvalidatedFrameLength),
        ];
    }

    public static byte[] EncodeIndication(byte sequenceNumber, IndicationBody body)
    {
        var header = BuildUnvalidatedHeader(ReadIndicationCommand, sequenceNumber);
        var destination = Concat(
            [(byte)DeconzAddressMode.Nwk],
            LittleEndian.Bytes(CoordinatorNetworkAddress),
            [CoordinatorEndpoint]
        );
        byte[] source = [(byte)DeconzAddressMode.Nwk, .. LittleEndian.Bytes(body.SourceNwk), body.SourceEndpoint];
        byte[] profileAndCluster = [.. LittleEndian.Bytes(body.ProfileId), .. LittleEndian.Bytes(body.ClusterId)];
        var asdu = Concat(LittleEndian.Bytes((ushort)body.Asdu.Length), body.Asdu);
        byte[] reservedAndLinkQuality = [0x00, 0x00, NoRssiLinkQuality];
        return Concat(header, destination, source, profileAndCluster, asdu, reservedAndLinkQuality);
    }

    private static byte[] EncodeConfirm(byte sequenceNumber, ConfirmBody body)
    {
        var header = Concat(BuildUnvalidatedHeader(ReadConfirmCommand, sequenceNumber), [body.RequestId]);
        var destination = Concat(
            [(byte)DeconzAddressMode.Nwk],
            LittleEndian.Bytes(body.DestinationNetworkAddress),
            [body.DestinationEndpoint]
        );
        byte[] sourceEndpointAndStatus = [body.SourceEndpoint, SuccessStatus];
        return Concat(header, destination, sourceEndpointAndStatus);
    }

    // The frameLength/payloadLength fields are never validated by the real client-side decoder, and
    // the trailing device-state byte is captured but nothing downstream reads it -- it's a distinct
    // field from (and unrelated to) the DeviceState poll response's own available-flags.
    private static byte[] BuildUnvalidatedHeader(byte command, byte sequenceNumber)
    {
        const byte unusedDeviceState = 0x00;
        return Concat(
            [command, sequenceNumber, SuccessStatus],
            LittleEndian.Bytes(UnvalidatedFrameLength),
            LittleEndian.Bytes(UnvalidatedPayloadLength),
            [unusedDeviceState]
        );
    }

    private static byte[] Concat(params byte[][] segments)
    {
        return segments.SelectMany(segment => segment).ToArray();
    }
}
