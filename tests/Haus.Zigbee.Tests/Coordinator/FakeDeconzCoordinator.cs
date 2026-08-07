using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Tests.Coordinator;

// A single-transport stand-in for the deCONZ dongle for driving the whole coordinator façade.
// Unlike FakeDeconzDongle (two faces for testing the poll loop and sender in isolation), the
// façade owns a single serial line, so this answers every command kind — read/write parameter,
// device-state poll, read-indication, and APS-data request — on one stream. It stays coherent
// because the coordinator's background loop drives each poll cycle (and any interview send it
// synchronously triggers) to completion before delaying, so channel round-trips never overlap.
internal class FakeDeconzCoordinator : ISerialTransport
{
    private const byte ReadParameterCommand = 0x0A;
    private const byte WriteParameterCommand = 0x0B;
    private const byte DeviceStateCommand = 0x07;
    private const byte ReadIndicationCommand = 0x17;
    private const byte ApsDataRequestCommand = 0x12;
    private const byte IndicationAvailable = 0x08;
    private const byte NothingAvailable = 0x00;
    private const byte WriteParameterSuccess = 0x00;
    private const int CommandOffset = 0;
    private const int SequenceNumberOffset = 1;
    private const int ParameterIdOffset = 7;
    private const int WriteParameterValueOffset = 8;
    private const int ChecksumLength = 2;

    private readonly object _lock = new();
    private readonly Dictionary<byte, byte[]> _parameters = new();
    private readonly Queue<IndicationBody> _indications = new();
    private readonly Dictionary<int, IndicationBody> _releaseOnApsRequest = new();
    private readonly List<WrittenParameter> _writtenParameters = new();
    private readonly List<byte[]> _apsRequestFrames = new();
    private int _apsRequestCount;
    private int _deviceStatePollCount;
    private byte[] _pendingResponse = Array.Empty<byte>();

    public void SetParameter(byte parameterId, byte[] value)
    {
        lock (_lock)
            _parameters[parameterId] = value;
    }

    public void InjectIndication(IndicationBody body)
    {
        lock (_lock)
            _indications.Enqueue(body);
    }

    public void ReleaseAfterApsRequest(int apsRequestIndex, IndicationBody body)
    {
        lock (_lock)
            _releaseOnApsRequest[apsRequestIndex] = body;
    }

    public IReadOnlyList<WrittenParameter> WrittenParameters
    {
        get
        {
            lock (_lock)
                return _writtenParameters.ToList();
        }
    }

    public IReadOnlyList<byte[]> ApsRequestFrames
    {
        get
        {
            lock (_lock)
                return _apsRequestFrames.ToList();
        }
    }

    public int DeviceStatePollCount
    {
        get
        {
            lock (_lock)
                return _deviceStatePollCount;
        }
    }

    public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

    public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
    {
        lock (_lock)
            _pendingResponse = BuildResponse(Decode(buffer));
        return Task.CompletedTask;
    }

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
    {
        lock (_lock)
        {
            var response = _pendingResponse;
            _pendingResponse = Array.Empty<byte>();
            response.CopyTo(buffer);
            return Task.FromResult(response.Length);
        }
    }

    private byte[] BuildResponse(byte[] request)
    {
        var sequenceNumber = request[SequenceNumberOffset];
        return request[CommandOffset] switch
        {
            ReadParameterCommand => ReadParameterResponse(sequenceNumber, request[ParameterIdOffset]),
            WriteParameterCommand => WriteParameterResponse(request),
            DeviceStateCommand => DeviceStateResponse(sequenceNumber),
            ReadIndicationCommand => IndicationResponse(sequenceNumber),
            ApsDataRequestCommand => ApsDataRequestResponse(request),
            _ => Array.Empty<byte>(),
        };
    }

    private byte[] ReadParameterResponse(byte sequenceNumber, byte parameterId)
    {
        var value = _parameters.TryGetValue(parameterId, out var stored) ? stored : Array.Empty<byte>();
        return DeconzFrames.Framed(
            ReadParameterFrame.Encode(new ReadParameterRequest(sequenceNumber, parameterId, value))
        );
    }

    private byte[] WriteParameterResponse(byte[] request)
    {
        var parameterId = request[ParameterIdOffset];
        _writtenParameters.Add(new WrittenParameter(parameterId, request[WriteParameterValueOffset..]));
        var response = new byte[]
        {
            WriteParameterCommand,
            request[SequenceNumberOffset],
            WriteParameterSuccess,
            0x08,
            0x00,
            0x01,
            0x00,
            parameterId,
        };
        return DeconzFrames.Framed(response);
    }

    private byte[] DeviceStateResponse(byte sequenceNumber)
    {
        _deviceStatePollCount++;
        var deviceState = _indications.Count > 0 ? IndicationAvailable : NothingAvailable;
        return DeconzFrames.DeviceState(sequenceNumber, deviceState);
    }

    private byte[] IndicationResponse(byte sequenceNumber)
    {
        var body = _indications.Count > 0 ? _indications.Dequeue() : null;
        return body is null ? Array.Empty<byte>() : DeconzFrames.Indication(sequenceNumber, body);
    }

    private byte[] ApsDataRequestResponse(byte[] request)
    {
        _apsRequestFrames.Add(request);
        var apsRequestIndex = _apsRequestCount++;
        if (_releaseOnApsRequest.TryGetValue(apsRequestIndex, out var body))
            _indications.Enqueue(body);
        return DeconzFrames.RequestAck(request[SequenceNumberOffset]);
    }

    private static byte[] Decode(ReadOnlyMemory<byte> framed)
    {
        var frame = new SlipDecoder().Decode(framed.ToArray())[0];
        return frame[..^ChecksumLength];
    }

    public void Dispose() { }

    public record WrittenParameter(byte ParameterId, byte[] Value);
}
