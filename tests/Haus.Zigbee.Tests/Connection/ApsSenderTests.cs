using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Transport;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class ApsSenderTests
{
    private const byte SuccessStatus = 0x00;
    private const byte NwkAddressMode = 0x02;
    private const byte ConfirmAvailable = 0x04;

    private readonly ScriptedSerialTransport _senderTransport = new();
    private readonly ScriptedSerialTransport _pollTransport = new();
    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;

    public ApsSenderTests()
    {
        _pollLoop = new ApsPollLoop(new DeconzChannel(_pollTransport));
        _sender = new ApsSender(_pollLoop, new DeconzChannel(_senderTransport));
    }

    [Fact]
    public async Task WhenSendingThenItReturnsTheConfirmThatMatchesTheRequestId()
    {
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x42)));

        var sendTask = _sender.SendAsync(Request(requestId: 0x42), CancellationToken.None);
        await _pollLoop.PollOnceAsync(CancellationToken.None);
        var confirm = await sendTask;

        Assert.Equal(0x42, confirm.RequestId);
    }

    [Fact]
    public async Task WhenAConfirmForAnotherRequestArrivesThenTheSendKeepsWaitingForItsOwn()
    {
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x99)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 2, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 3, requestId: 0x42)));

        var sendTask = _sender.SendAsync(Request(requestId: 0x42), CancellationToken.None);
        await _pollLoop.PollOnceAsync(CancellationToken.None);
        Assert.False(sendTask.IsCompleted);

        await _pollLoop.PollOnceAsync(CancellationToken.None);
        Assert.Equal(0x42, (await sendTask).RequestId);
    }

    [Fact]
    public async Task WhenDisposedThenAConfirmNoLongerCompletesAPendingSend()
    {
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x42)));

        var sendTask = _sender.SendAsync(Request(requestId: 0x42), CancellationToken.None);
        _sender.Dispose();
        await _pollLoop.PollOnceAsync(CancellationToken.None);

        var settled = await Task.WhenAny(sendTask, Task.Delay(200));
        Assert.NotEqual(sendTask, settled);
    }

    [Fact]
    public async Task WhenNoConfirmEverArrivesThenSendAsyncTimesOutInsteadOfHangingForever()
    {
        var timingOutSender = new ApsSender(
            _pollLoop,
            new DeconzChannel(_senderTransport),
            confirmTimeout: TimeSpan.FromMilliseconds(50)
        );
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));

        var sendTask = timingOutSender.SendAsync(Request(requestId: 0x42), CancellationToken.None);
        var completed = await Task.WhenAny(sendTask, Task.Delay(TimeSpan.FromSeconds(5)));

        Assert.Same(sendTask, completed);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => sendTask);
    }

    [Fact]
    public async Task SendAsync_CalledConcurrentlyFromManyThreads_NeverReusesASequenceNumberWithinOneWraparound()
    {
        // Real OS threads racing a Barrier, matching the pattern that already caught the same
        // class of bug in ByteSequenceCounterTests: _sequenceNumber++ happens before
        // SendAndReceiveAsync's mutex is even acquired, so concurrent callers can race on it
        // regardless of how well-synchronized the channel itself is.
        const int callCount = 64;
        var transport = new AutoAckingTransport();
        var sender = new ApsSender(
            _pollLoop,
            new DeconzChannel(transport),
            confirmTimeout: TimeSpan.FromMilliseconds(50)
        );
        using var barrier = new Barrier(callCount);

        var threads = Enumerable
            .Range(0, callCount)
            .Select(i => new Thread(() =>
            {
                barrier.SignalAndWait();
                try
                {
                    sender.SendAsync(Request(requestId: (byte)i), CancellationToken.None).GetAwaiter().GetResult();
                }
                catch (OperationCanceledException) { }
            }))
            .ToList();
        foreach (var thread in threads)
            thread.Start();
        foreach (var thread in threads)
            thread.Join();

        var sequenceNumbers = ExtractSequenceNumbers(transport.WrittenBytes);
        Assert.Equal(callCount, sequenceNumbers.Distinct().Count());
    }

    private static List<byte> ExtractSequenceNumbers(IReadOnlyList<byte> writtenBytes)
    {
        var frames = new SlipDecoder().Decode(writtenBytes.ToArray());
        return frames.Select(frame => frame[1]).ToList();
    }

    private class AutoAckingTransport : ISerialTransport
    {
        private readonly List<byte> _writtenBytes = new();
        private readonly Queue<byte> _incoming = new();
        private readonly object _lock = new();

        public IReadOnlyList<byte> WrittenBytes
        {
            get
            {
                lock (_lock)
                    return _writtenBytes.ToList();
            }
        }

        public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            var written = buffer.ToArray();
            var frame = new SlipDecoder().Decode(written)[0];
            var sequenceNumber = frame[1];
            var ack = Framed(new byte[] { 0x12, sequenceNumber, SuccessStatus, 0x00, 0x00 });

            lock (_lock)
            {
                _writtenBytes.AddRange(written);
                foreach (var b in ack)
                    _incoming.Enqueue(b);
            }
            return Task.CompletedTask;
        }

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            lock (_lock)
            {
                var count = 0;
                while (count < buffer.Length && _incoming.Count > 0)
                    buffer.Span[count++] = _incoming.Dequeue();
                return Task.FromResult(count);
            }
        }

        public void Dispose() { }
    }

    [Fact]
    public void WhenSendingThenTheCommandFrameUsesTheSendersOwnSequenceNumber()
    {
        // The send is never awaited/completed and no response is ever queued, so its internal
        // wait would otherwise spin forever -- bound it (deliberately not disposed: it must
        // outlive this method to still fire) so the abandoned task actually terminates instead of
        // pinning a thread pool thread for the rest of the process's life. The write itself still
        // happens synchronously before this returns, which is all this test checks.
        var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        _ = _sender.SendAsync(Request(requestId: 0x42, sequenceNumber: 99), timeout.Token);

        var expected = Framed(ApsDataRequestFrameCodec.Encode(Request(requestId: 0x42, sequenceNumber: 0)));
        Assert.Equal(expected, _senderTransport.WrittenBytes);
    }

    private static ApsDataRequestFrame Request(byte requestId, byte sequenceNumber = 0)
    {
        return new ApsDataRequestFrame(
            SequenceNumber: sequenceNumber,
            RequestId: requestId,
            Destination: ApsDestination.Nwk(0x1234, 0x01),
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            SourceEndpoint: 0x01,
            AsduPayload: new byte[] { 0x01 },
            TxOptions: 0x00,
            Radius: 0x00
        );
    }

    private static byte[] DeconzAck(byte sequenceNumber)
    {
        return new byte[] { 0x12, sequenceNumber, SuccessStatus, 0x00, 0x00 };
    }

    private static byte[] DeviceStateResponse(byte sequenceNumber, byte deviceState)
    {
        return new byte[] { 0x07, sequenceNumber, 0x00, 0x00, 0x00, deviceState };
    }

    private static byte[] ConfirmResponse(byte sequenceNumber, byte requestId)
    {
        var header = new byte[] { 0x04, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var requestAndAddress = new byte[] { requestId, NwkAddressMode, 0x34, 0x12 };
        var endpointsAndStatus = new byte[] { 0x01, 0x01, 0x00 };
        return header.Concat(requestAndAddress).Concat(endpointsAndStatus).ToArray();
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzChecksum.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
