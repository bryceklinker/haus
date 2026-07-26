using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Transport;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class PermitJoinControllerTests
{
    private const byte PermitJoinParameterId = 0x21;

    private readonly FakeCoordinator _coordinator = new();
    private readonly PermitJoinController _controller;

    public PermitJoinControllerTests()
    {
        _controller = new PermitJoinController(new DeconzChannel(_coordinator));
    }

    [Fact]
    public async Task WhenEnablingPermitJoinThenWritesIndefiniteDurationToThePermitJoinParameter()
    {
        await _controller.SetPermitJoinAsync(true, CancellationToken.None);

        var request = _coordinator.LastWriteRequest;
        Assert.Equal(PermitJoinParameterId, request.ParameterId);
        Assert.Equal(new byte[] { 0xFF }, request.Value);
    }

    [Fact]
    public async Task WhenDisablingPermitJoinThenWritesZeroDurationToThePermitJoinParameter()
    {
        await _controller.SetPermitJoinAsync(false, CancellationToken.None);

        var request = _coordinator.LastWriteRequest;
        Assert.Equal(PermitJoinParameterId, request.ParameterId);
        Assert.Equal(new byte[] { 0x00 }, request.Value);
    }

    // A fake deCONZ coordinator over the serial seam: it records each write-parameter request it
    // is sent, then answers with a success response echoing the request's sequence number back so
    // DeconzChannel can correlate it — exactly as real hardware would.
    private sealed class FakeCoordinator : ISerialTransport
    {
        private const int SequenceNumberIndex = 1;
        private const int ParameterIdIndex = 7;
        private const int ValueIndex = 8;
        private const byte WriteParameterCommandId = 0x0B;
        private const byte SuccessStatus = 0x00;

        private readonly Queue<byte> _incoming = new();

        public WrittenRequest LastWriteRequest { get; private set; } = null!;

        public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            var frame = new SlipDecoder().Decode(buffer.ToArray())[0][..^2];
            LastWriteRequest = new WrittenRequest(frame[ParameterIdIndex], frame[ValueIndex..]);
            Respond(frame[SequenceNumberIndex], frame[ParameterIdIndex]);
            return Task.CompletedTask;
        }

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            var count = 0;
            while (count < buffer.Length && _incoming.Count > 0)
                buffer.Span[count++] = _incoming.Dequeue();
            return Task.FromResult(count);
        }

        private void Respond(byte sequenceNumber, byte parameterId)
        {
            var response = new byte[]
            {
                WriteParameterCommandId,
                sequenceNumber,
                SuccessStatus,
                0x08,
                0x00,
                0x01,
                0x00,
                parameterId,
            };
            Enqueue(Framed(response));
        }

        private void Enqueue(byte[] bytes)
        {
            foreach (var value in bytes)
                _incoming.Enqueue(value);
        }

        private static byte[] Framed(byte[] frame)
        {
            var checksum = DeconzCrc.Compute(frame);
            var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
            return new SlipEncoder().Encode(withChecksum.ToArray());
        }
    }

    private sealed record WrittenRequest(byte ParameterId, byte[] Value);
}
