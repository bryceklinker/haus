using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Transport;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class DeconzConnectionTests
{
    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1C;

    private readonly FakeCoordinator _coordinator = new();
    private readonly DeconzConnection _connection;

    public DeconzConnectionTests()
    {
        _connection = new DeconzConnection(new DeconzChannel(_coordinator));
    }

    [Fact]
    public async Task WhenConnectingThenReadsBackTheExistingNetworkConfig()
    {
        var expectedMacAddress = new IeeeAddress(0x00212EFFFF001234);
        _coordinator.OnRead(MacAddressParameterId, LittleEndianUInt64(expectedMacAddress.Value));
        _coordinator.OnRead(PanIdParameterId, LittleEndianUInt16(0x1A62));
        _coordinator.OnRead(ChannelParameterId, new byte[] { 0x0F });

        var config = await _connection.ConnectAsync(CancellationToken.None);

        Assert.Equal(new NetworkConfig(expectedMacAddress, 0x1A62, 0x0F), config);
    }

    [Fact]
    public async Task WhenConnectingThenEachParameterReadUsesADistinctSequenceNumber()
    {
        _coordinator.OnRead(MacAddressParameterId, LittleEndianUInt64(0));
        _coordinator.OnRead(PanIdParameterId, LittleEndianUInt16(0));
        _coordinator.OnRead(ChannelParameterId, new byte[] { 0 });

        await _connection.ConnectAsync(CancellationToken.None);

        var sequenceNumbers = _coordinator.Requests.Select(SequenceNumberOf).ToArray();
        Assert.Equal(3, sequenceNumbers.Length);
        Assert.Equal(sequenceNumbers.Length, sequenceNumbers.Distinct().Count());
    }

    private static byte SequenceNumberOf(byte[] request) => request[1];

    private static byte[] LittleEndianUInt64(ulong value)
    {
        var bytes = new byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes, value);
        return bytes;
    }

    private static byte[] LittleEndianUInt16(ushort value)
    {
        var bytes = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);
        return bytes;
    }

    // A fake deCONZ coordinator over the serial seam: it answers each read-parameter request
    // with the value it was told to hold for that parameter, echoing the request's sequence
    // number back so DeconzChannel can correlate the response — exactly as real hardware would.
    private class FakeCoordinator : ISerialTransport
    {
        private const int SequenceNumberIndex = 1;
        private const int ParameterIdIndex = 7;

        private readonly Dictionary<byte, byte[]> _valuesByParameterId = new();
        private readonly Queue<byte> _incoming = new();
        private readonly List<byte[]> _requests = new();

        public IReadOnlyList<byte[]> Requests => _requests;

        public void OnRead(byte parameterId, byte[] value)
        {
            _valuesByParameterId[parameterId] = value;
        }

        public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            var request = DecodeRequest(buffer.ToArray());
            _requests.Add(request);
            Respond(request);
            return Task.CompletedTask;
        }

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            var count = 0;
            while (count < buffer.Length && _incoming.Count > 0)
                buffer.Span[count++] = _incoming.Dequeue();
            return Task.FromResult(count);
        }

        private static byte[] DecodeRequest(byte[] written)
        {
            var frameWithChecksum = new SlipDecoder().Decode(written)[0];
            return frameWithChecksum[..^2];
        }

        private void Respond(byte[] request)
        {
            var response = new ReadParameterRequest(
                request[SequenceNumberIndex],
                request[ParameterIdIndex],
                _valuesByParameterId[request[ParameterIdIndex]]
            );
            Enqueue(Framed(ReadParameterFrame.Encode(response)));
        }

        private void Enqueue(byte[] bytes)
        {
            foreach (var value in bytes)
                _incoming.Enqueue(value);
        }

        private static byte[] Framed(byte[] frame)
        {
            var checksum = DeconzChecksum.Compute(frame);
            var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
            return new SlipEncoder().Encode(withChecksum.ToArray());
        }
    }
}
