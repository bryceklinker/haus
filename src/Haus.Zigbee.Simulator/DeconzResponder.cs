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
    private const byte ApsDataRequestCommand = 0x12;
    private const byte ConfirmAvailable = 0x04;
    private const byte IndicationAvailable = 0x08;
    private const byte NothingAvailable = 0x00;
    private const int SequenceNumberOffset = 1;
    private const int ParameterIdOffset = 7;
    private const int WriteParameterValueOffset = 8;
    private const int RequestIdOffset = 7;

    // WriteParameter response is a fixed 8-byte frame (command+seq+status+frameLength(2)+
    // payloadLength(2)+parameterId) when the written value is just the parameter id -- no extra
    // bytes echoed back.
    private const ushort WriteParameterResponseFrameLength = 8;
    private const ushort WriteParameterResponsePayloadLength = 1;

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

    private readonly DeconzParameterStore _parameterStore = new();
    private readonly DeconzMessageQueue _messageQueue = new();
    private readonly DeconzInterviewScript _interviewScript = new();
    private readonly ConcurrentQueue<byte[]> _sentApsRequests = new();
    private int _networkAddressCounter;

    public void SetParameter(byte parameterId, byte[] value)
    {
        _parameterStore.Set(parameterId, value);
    }

    public void EnqueueIndication(IndicationBody body)
    {
        _messageQueue.EnqueueIndication(body);
    }

    public void ReleaseAfterApsRequest(ushort networkAddress, int step, Func<byte[], IndicationBody> bodyFactory)
    {
        _interviewScript.ReleaseAfterApsRequest(networkAddress, step, bodyFactory);
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
        return _interviewScript.GetApsRequestCountForDevice(networkAddress);
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
            DeconzFrameEncoder.ReadIndicationCommand => "ReadIndication",
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
            DeconzFrameEncoder.ReadIndicationCommand => IndicationResponse(sequenceNumber),
            DeconzFrameEncoder.ReadConfirmCommand => ConfirmResponse(sequenceNumber),
            ApsDataRequestCommand => ApsDataRequestResponse(request),
            _ => [],
        };
    }

    private byte[] ReadParameterResponse(byte sequenceNumber, byte parameterId)
    {
        var value = _parameterStore.Get(parameterId);
        return ReadParameterFrame.Encode(new ReadParameterRequest(sequenceNumber, parameterId, value));
    }

    private byte[] WriteParameterResponse(byte[] request)
    {
        var parameterId = request[ParameterIdOffset];
        _parameterStore.Set(parameterId, request[WriteParameterValueOffset..]);
        return
        [
            WriteParameterCommand,
            request[SequenceNumberOffset],
            DeconzFrameEncoder.SuccessStatus,
            .. LittleEndian.Bytes(WriteParameterResponseFrameLength),
            .. LittleEndian.Bytes(WriteParameterResponsePayloadLength),
            parameterId,
        ];
    }

    private byte[] DeviceStateResponse(byte sequenceNumber)
    {
        byte deviceState = NothingAvailable;
        if (_messageQueue.HasIndication)
            deviceState |= IndicationAvailable;
        if (_messageQueue.HasConfirm)
            deviceState |= ConfirmAvailable;

        return
        [
            DeviceStateCommand,
            sequenceNumber,
            DeconzFrameEncoder.SuccessStatus,
            .. LittleEndian.Bytes(DeconzFrameEncoder.UnvalidatedFrameLength),
            deviceState,
        ];
    }

    private byte[] IndicationResponse(byte sequenceNumber)
    {
        return _messageQueue.TryDequeueIndication(out var body)
            ? DeconzFrameEncoder.EncodeIndication(sequenceNumber, body!)
            : [];
    }

    private byte[] ConfirmResponse(byte sequenceNumber)
    {
        return _messageQueue.TryDequeueConfirm(out var body)
            ? DeconzFrameEncoder.EncodeConfirm(sequenceNumber, body!)
            : [];
    }

    private byte[] ApsDataRequestResponse(byte[] request)
    {
        _sentApsRequests.Enqueue(request);
        var networkAddress = ExtractDestinationNetworkAddress(request);
        _messageQueue.EnqueueConfirm(
            new ConfirmBody(
                request[RequestIdOffset],
                networkAddress,
                request[DestinationEndpointOffset],
                request[SourceEndpointOffset]
            )
        );
        var releasedIndication = _interviewScript.RecordApsRequestAndTryRelease(networkAddress, request);
        if (releasedIndication is not null)
            _messageQueue.EnqueueIndication(releasedIndication);

        return
        [
            ApsDataRequestCommand,
            request[SequenceNumberOffset],
            DeconzFrameEncoder.SuccessStatus,
            .. LittleEndian.Bytes(DeconzFrameEncoder.UnvalidatedFrameLength),
        ];
    }

    public static byte[] EncodeIndication(byte sequenceNumber, IndicationBody body)
    {
        return DeconzFrameEncoder.EncodeIndication(sequenceNumber, body);
    }
}
