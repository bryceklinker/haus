using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Tests.Coordinator;
using Haus.Zigbee.Tests.Support;
using Haus.Zigbee.Tests.Transport;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class DeconzChannelTests
{
    private readonly FakeSerialTransport _transport = new();
    private readonly CapturingLoggerFactory _loggerFactory = new();
    private readonly DeconzChannel _channel;

    public DeconzChannelTests()
    {
        _channel = new DeconzChannel(_transport, _loggerFactory);
    }

    [Fact]
    public async Task WhenSendingAFrameThenTheSendAndReceiveAreLoggedAtDebugLevel()
    {
        var command = new byte[] { 0x0A, 0x05, 0x01 };
        _transport.FeedIncoming(DeconzFrames.Framed(new byte[] { 0x0A, 0x05, 0x00 }));

        await _channel.SendAndReceiveAsync(command, CancellationToken.None);

        var debugEntries = _loggerFactory
            .Entries.Where(entry => entry.Category == typeof(DeconzChannel).FullName && entry.Level == LogLevel.Debug)
            .ToList();
        Assert.True(debugEntries.Count >= 2);
    }

    [Fact]
    public async Task WhenSendingAFrameThenItIsChecksumAppendedAndSlipEncodedOntoTheTransport()
    {
        var command = new byte[] { 0x0A, 0x05, 0x01 };
        _transport.FeedIncoming(DeconzFrames.Framed(new byte[] { 0x0A, 0x05, 0x00 }));

        await _channel.SendAndReceiveAsync(command, CancellationToken.None);

        Assert.Equal(DeconzFrames.Framed(command), _transport.WrittenBytes);
    }

    [Fact]
    public async Task WhenResponsesArriveThenTheOneMatchingTheSentSequenceNumberIsReturned()
    {
        var command = new byte[] { 0x0A, 0x05, 0x01 };
        _transport.FeedIncoming(DeconzFrames.Framed(new byte[] { 0x0A, 0x09, 0xFF }));
        _transport.FeedIncoming(DeconzFrames.Framed(new byte[] { 0x0A, 0x05, 0x42 }));

        var response = await _channel.SendAndReceiveAsync(command, CancellationToken.None);

        Assert.Equal(new byte[] { 0x0A, 0x05, 0x42 }, response);
    }

    [Fact]
    public async Task WhenCancellationArrivesAfterANonMatchingReadThenTheWaitThrowsInsteadOfReturningEmptyBytes()
    {
        using var cancellation = new CancellationTokenSource();
        var transport = new CancellingTransport(cancellation);
        var channel = new DeconzChannel(transport);
        var command = new byte[] { 0x0A, 0x05, 0x01 };

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            channel.SendAndReceiveAsync(command, cancellation.Token)
        );
    }

    [Fact]
    public async Task WhenTheTransportReadNeverCompletesThenSendAndReceiveAsyncTimesOutInsteadOfHangingForever()
    {
        var transport = new HangUntilDisposedTransport();
        var channel = new DeconzChannel(transport, roundTripTimeout: TimeSpan.FromMilliseconds(50));
        var command = new byte[] { 0x0A, 0x05, 0x01 };

        using var testTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await Assert.ThrowsAsync<SerialTransportTimeoutException>(() =>
            channel.SendAndReceiveAsync(command, testTimeout.Token)
        );

        Assert.Equal(1, transport.DisposeCallCount);
    }

    // If the mutex were never released, the second call's WaitAsync would block until the
    // 2-second CancellationTokenSource below cancels it, surfacing as an OperationCanceledException
    // instead of the ObjectDisposedException a fast-failing second round trip against the
    // already-disposed transport actually produces.
    [Fact]
    public async Task WhenARoundTripTimesOutThenTheMutexIsReleasedForTheNextCall()
    {
        var transport = new HangUntilDisposedTransport();
        var channel = new DeconzChannel(transport, roundTripTimeout: TimeSpan.FromMilliseconds(50));
        var command = new byte[] { 0x0A, 0x05, 0x01 };
        await Assert.ThrowsAsync<SerialTransportTimeoutException>(() =>
            channel.SendAndReceiveAsync(command, CancellationToken.None)
        );

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await Assert.ThrowsAsync<ObjectDisposedException>(() => channel.SendAndReceiveAsync(command, timeout.Token));
    }

    // Reproduces a race the mutex depends on for safety: the caller's own token fires before the
    // channel's internal round-trip timeout, while the transport underneath is genuinely hung (not
    // just abandoned by an impatient caller). If SendAndReceiveAsync returned as soon as the caller
    // cancelled -- without finding out whether the transport was actually still wedged -- the
    // mutex would still release via the outer finally, and the NEXT caller could start writing
    // into the same never-disposed, still-hung transport underneath it.
    [Fact]
    public async Task WhenTheCallerCancelsBeforeTheRoundTripTimeoutButTheTransportIsHungThenItIsStillDisposed()
    {
        var transport = new HangUntilDisposedTransport();
        var channel = new DeconzChannel(transport, roundTripTimeout: TimeSpan.FromMilliseconds(200));
        var command = new byte[] { 0x0A, 0x05, 0x01 };

        using var callerTimeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(20));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            channel.SendAndReceiveAsync(command, callerTimeout.Token)
        );

        Assert.Equal(1, transport.DisposeCallCount);

        using var nextCallTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            channel.SendAndReceiveAsync(command, nextCallTimeout.Token)
        );
    }

    [Fact]
    public async Task WhenTwoSendsOverlapOnTheSameChannelThenEachCallerGetsItsOwnMatchingResponse()
    {
        var transport = new AutoRespondingTransport();
        var channel = new DeconzChannel(transport);
        var commandA = new byte[] { 0x0a, 0x05, 0x01 };
        var commandB = new byte[] { 0x0a, 0x06, 0x02 };
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        var callA = channel.SendAndReceiveAsync(commandA, timeout.Token);
        var callB = channel.SendAndReceiveAsync(commandB, timeout.Token);
        var responseA = await callA;
        var responseB = await callB;

        Assert.Equal(new byte[] { 0x0a, 0x05, 0xaa }, responseA);
        Assert.Equal(new byte[] { 0x0a, 0x06, 0xaa }, responseB);
    }

    // Reproduces the race where cancellation arrives after a read returns with no matching frame:
    // it cancels the shared token from inside ReadAsync itself (simulating cancellation landing
    // between that non-matching read and the loop's next iteration check) and reports zero bytes
    // read, the same as a real transport with nothing new on the wire yet.
    private class CancellingTransport(CancellationTokenSource cancellation) : ISerialTransport
    {
        public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) => Task.CompletedTask;

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            cancellation.Cancel();
            return Task.FromResult(0);
        }

        public void Dispose() { }
    }

    // Models a real deCONZ dongle that has dropped mid round-trip: the read never completes on its
    // own, the same way a torn-down Linux SerialPort's pending read hangs instead of throwing.
    // Only Dispose ever unsticks it, which is exactly the recovery DeconzChannel must perform.
    private class HangUntilDisposedTransport : ISerialTransport
    {
        private readonly TaskCompletionSource<int> _readHang = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int DisposeCallCount { get; private set; }

        public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) => Task.CompletedTask;

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token) => _readHang.Task;

        public void Dispose()
        {
            DisposeCallCount++;
            _readHang.TrySetException(
                new ObjectDisposedException(nameof(HangUntilDisposedTransport), "The port is closed.")
            );
        }
    }

    // Echoes a response for whatever command+sequence-number was just written, after yielding once
    // -- the yield opens a genuine interleaving window for a second concurrent SendAndReceiveAsync
    // call on the same channel/transport, the way two real callers racing on one serial line would.
    private class AutoRespondingTransport : ISerialTransport
    {
        private readonly List<byte> _writtenBytes = new();
        private readonly Queue<byte> _incomingBytes = new();

        public Task OpenAsync(CancellationToken token) => Task.CompletedTask;

        public Task CloseAsync(CancellationToken token) => Task.CompletedTask;

        public async Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            var written = buffer.ToArray();
            _writtenBytes.AddRange(written);
            await Task.Yield();

            var decoded = new SlipDecoder().Decode(written)[0];
            var command = decoded[..^2];
            var response = DeconzFrames.Framed(new byte[] { command[0], command[1], 0xaa });
            foreach (var b in response)
                _incomingBytes.Enqueue(b);
        }

        public async Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            while (_incomingBytes.Count == 0 && !token.IsCancellationRequested)
                await Task.Yield();

            var count = 0;
            while (count < buffer.Length && _incomingBytes.Count > 0)
                buffer.Span[count++] = _incomingBytes.Dequeue();
            return count;
        }

        public void Dispose() { }
    }
}
