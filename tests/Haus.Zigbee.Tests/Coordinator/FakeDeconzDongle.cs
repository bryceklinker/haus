using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Tests.Coordinator;

// A scripted stand-in for the deCONZ dongle at the serial seam. It answers the poll loop's
// device-state/read-indication requests and the sender's APS-data requests, and — crucially —
// only makes a response indication available AFTER the interview has actually written the request
// that triggers it (keyed by send order). That guarantees the interview has registered its
// correlation for a response before that response can be drained, so the interview's asynchronous
// step continuations never race the poll loop. The poll loop and the sender each get their own
// face because deCONZChannel correlates responses by sequence number and cannot share one stream.
internal class FakeDeconzDongle
{
    private const byte DeviceStateCommand = 0x07;
    private const byte ReadIndicationCommand = 0x17;
    private const byte ApsDataRequestCommand = 0x12;
    private const byte IndicationAvailable = 0x08;
    private const byte NothingAvailable = 0x00;
    private const int SequenceNumberOffset = 1;
    private const int ChecksumLength = 2;

    private readonly object _lock = new();
    private readonly Queue<IndicationBody> _indications = new();
    private readonly Dictionary<int, IndicationBody> _releaseOnSend = new();
    private int _sendCount;

    public FakeDeconzDongle()
    {
        PollTransport = new PollFace(this);
        SendTransport = new SendFace(this);
    }

    public ISerialTransport PollTransport { get; }
    public ISerialTransport SendTransport { get; }

    public void InjectIndication(IndicationBody body)
    {
        lock (_lock)
            _indications.Enqueue(body);
    }

    public void ReleaseAfterSend(int sendIndex, IndicationBody body)
    {
        lock (_lock)
            _releaseOnSend[sendIndex] = body;
    }

    private void OnRequestSent()
    {
        lock (_lock)
        {
            var sendIndex = _sendCount++;
            if (_releaseOnSend.TryGetValue(sendIndex, out var body))
                _indications.Enqueue(body);
        }
    }

    private bool HasIndication()
    {
        lock (_lock)
            return _indications.Count > 0;
    }

    private IndicationBody? DequeueIndication()
    {
        lock (_lock)
            return _indications.Count > 0 ? _indications.Dequeue() : null;
    }

    private static byte[] DecodeRequest(byte[] framed)
    {
        var frame = new SlipDecoder().Decode(framed)[0];
        return frame[..^ChecksumLength];
    }

    private class PollFace(FakeDeconzDongle dongle) : StoredResponseTransport
    {
        protected override byte[] BuildResponse(byte[] request)
        {
            var sequenceNumber = request[SequenceNumberOffset];
            return request[0] switch
            {
                DeviceStateCommand => DeviceStateResponse(sequenceNumber),
                ReadIndicationCommand => IndicationResponse(sequenceNumber),
                _ => Array.Empty<byte>(),
            };
        }

        private byte[] DeviceStateResponse(byte sequenceNumber)
        {
            var deviceState = dongle.HasIndication() ? IndicationAvailable : NothingAvailable;
            return DeconzFrames.DeviceState(sequenceNumber, deviceState);
        }

        private byte[] IndicationResponse(byte sequenceNumber)
        {
            var body = dongle.DequeueIndication();
            return body is null ? Array.Empty<byte>() : DeconzFrames.Indication(sequenceNumber, body);
        }
    }

    private class SendFace(FakeDeconzDongle dongle) : StoredResponseTransport
    {
        protected override byte[] BuildResponse(byte[] request)
        {
            if (request[0] != ApsDataRequestCommand)
                return Array.Empty<byte>();

            dongle.OnRequestSent();
            return DeconzFrames.RequestAck(request[SequenceNumberOffset]);
        }
    }

    private abstract class StoredResponseTransport : ISerialTransport
    {
        private byte[] _pendingResponse = Array.Empty<byte>();

        protected abstract byte[] BuildResponse(byte[] request);

        public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            _pendingResponse = BuildResponse(DecodeRequest(buffer.ToArray()));
            return Task.CompletedTask;
        }

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            var response = _pendingResponse;
            _pendingResponse = Array.Empty<byte>();
            response.CopyTo(buffer);
            return Task.FromResult(response.Length);
        }

        public void Dispose() { }
    }
}
