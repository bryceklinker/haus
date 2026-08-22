using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Transport;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

// Reproduces the production deadlock class: a transport round trip hangs mid-poll instead of
// throwing, the way a dropped ConBee II dongle's torn-down SerialPort does on Linux. Proves the
// coordinator recovers on its own -- marking itself disconnected and rebuilding a fresh transport
// on the next connect attempt -- rather than staying permanently wedged.
public class ZigbeeCoordinatorReconnectTests
{
    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1C;

    // Short enough to keep these tests fast; long enough that no unrelated async hop in the
    // fakes below could ever be mistaken for the simulated hang.
    private static readonly TimeSpan ShortRoundTripTimeout = TimeSpan.FromMilliseconds(50);

    [Fact]
    public async Task WhenARoundTripHangsDuringPollingThenIsConnectedBecomesFalse()
    {
        var dongle = NewConfiguredDongle();
        var hangingTransport = new HangOnceOnReadTransport(dongle, hangOnReadCall: 4);
        using var coordinator = new ZigbeeCoordinator(
            () => hangingTransport,
            channelRoundTripTimeout: ShortRoundTripTimeout
        );

        await coordinator.ConnectAsync(CancellationToken.None);
        Assert.True(coordinator.IsConnected);

        await WaitUntilAsync(() => !coordinator.IsConnected);

        Assert.False(coordinator.IsConnected);
    }

    [Fact]
    public async Task WhenReconnectingAfterAHangThenANewTransportInstanceIsBuiltAndConnectsSuccessfully()
    {
        var firstDongle = NewConfiguredDongle();
        var hangingTransport = new HangOnceOnReadTransport(firstDongle, hangOnReadCall: 4);
        var secondDongle = NewConfiguredDongle();
        var transportsToBuild = new Queue<ISerialTransport>([hangingTransport, secondDongle]);
        var builtTransportCount = 0;
        using var coordinator = new ZigbeeCoordinator(
            () =>
            {
                builtTransportCount++;
                return transportsToBuild.Dequeue();
            },
            channelRoundTripTimeout: ShortRoundTripTimeout
        );

        await coordinator.ConnectAsync(CancellationToken.None);
        await WaitUntilAsync(() => !coordinator.IsConnected);

        await coordinator.ConnectAsync(CancellationToken.None);

        Assert.True(coordinator.IsConnected);
        Assert.Equal(2, builtTransportCount);
        await WaitUntilAsync(() => secondDongle.DeviceStatePollCount > 0);
    }

    // A physically-unplugged ConBee II dongle is documented to surface as a plain IOException from
    // .NET's SerialPort on Linux, not just ObjectDisposedException/SerialTransportTimeoutException.
    // Reproduces that failure mode directly (no hang involved) and proves it's classified as fatal
    // -- otherwise it would be swallowed as transient-and-retried-forever, reproducing this PR's
    // exact root-cause outage class through a different exception type.
    //
    // Unlike the hang-based tests above, a thrown (not hung) fatal failure can be detected
    // synchronously on the very first poll iteration -- there's no ShortRoundTripTimeout delay to
    // guarantee IsConnected still reads true immediately after ConnectAsync returns, so this only
    // asserts the eventual false transition, not a true-then-false sequence.
    [Fact]
    public async Task WhenTheTransportThrowsIOExceptionDuringPollingThenIsConnectedBecomesFalse()
    {
        var dongle = NewConfiguredDongle();
        var unpluggedTransport = new ThrowOnceOnReadTransport(dongle, throwOnReadCall: 4);
        using var coordinator = new ZigbeeCoordinator(
            () => unpluggedTransport,
            channelRoundTripTimeout: ShortRoundTripTimeout
        );

        await coordinator.ConnectAsync(CancellationToken.None);

        await WaitUntilAsync(() => !coordinator.IsConnected);

        Assert.False(coordinator.IsConnected);
    }

    // Before TransportComponents, ZigbeeCoordinator held eight separately-mutable fields
    // (_transport, _channel, _sender, ...) that a rebuild reassigned one at a time with no
    // synchronization: a concurrent caller could read a mismatched mix of old and new
    // collaborators, or a still-in-flight operation could be pulled out from under it mid-call.
    // This hammers SetPermitJoinAsync from one task while another task drives many rapid
    // rebuild cycles (every poll immediately hangs, forcing the coordinator to disconnect and
    // reconnect over and over), and asserts that only well-understood "the transport really is
    // dead" exceptions ever surface -- never a NullReferenceException or anything else that
    // would indicate torn/inconsistent component state.
    [Fact]
    public async Task WhenPublicCallsRaceRepeatedTransportRebuildsThenOnlyExpectedExceptionsSurface()
    {
        const int RebuildCycles = 15;
        var buildCount = 0;
        using var coordinator = new ZigbeeCoordinator(
            () =>
            {
                Interlocked.Increment(ref buildCount);
                return new HangOnceOnReadTransport(NewConfiguredDongle(), hangOnReadCall: 4);
            },
            channelRoundTripTimeout: ShortRoundTripTimeout
        );
        await coordinator.ConnectAsync(CancellationToken.None);

        using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var unexpectedExceptions = new ConcurrentBag<Exception>();

        // Mirrors ZigbeeHausBridge.TryConnectAsync's real behavior: a connect attempt that itself
        // hits the round-trip timeout under this much concurrent load is an expected, retryable
        // failure, not a bug -- the production supervisor already just logs it and tries again on
        // its next tick, so this loop does the same instead of treating it as unexpected.
        var reconnectLoop = Task.Run(async () =>
        {
            var completedCycles = 0;
            while (completedCycles < RebuildCycles && !stop.IsCancellationRequested)
            {
                await WaitUntilAsync(() => !coordinator.IsConnected);
                try
                {
                    await coordinator.ConnectAsync(CancellationToken.None);
                    completedCycles++;
                }
                catch (Exception ex) when (IsExpectedTransportFailure(ex)) { }
            }
        });

        var caller = Task.Run(async () =>
        {
            while (!reconnectLoop.IsCompleted && !stop.IsCancellationRequested)
            {
                try
                {
                    await coordinator.SetPermitJoinAsync(true, CancellationToken.None);
                }
                catch (Exception ex) when (IsExpectedTransportFailure(ex))
                {
                    // The transport genuinely died mid-call -- expected under this much churn.
                }
                catch (Exception ex)
                {
                    unexpectedExceptions.Add(ex);
                }
            }
        });

        await Task.WhenAll(reconnectLoop, caller);

        Assert.True(buildCount > RebuildCycles, "the rebuild loop never actually cycled through several transports");
        Assert.Empty(unexpectedExceptions);
    }

    private static bool IsExpectedTransportFailure(Exception ex) =>
        ex is ObjectDisposedException or SerialTransportTimeoutException or OperationCanceledException;

    private static FakeDeconzCoordinator NewConfiguredDongle()
    {
        var dongle = new FakeDeconzCoordinator();
        dongle.SetParameter(MacAddressParameterId, new byte[] { 0x22, 0x11, 0x00, 0xff, 0xff, 0x2e, 0x21, 0x00 });
        dongle.SetParameter(PanIdParameterId, new byte[] { 0x62, 0x1a });
        dongle.SetParameter(ChannelParameterId, new byte[] { 0x0f });
        return dongle;
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!condition() && !timeout.IsCancellationRequested)
            await Task.Delay(10);
        Assert.True(condition(), "condition was not met within the timeout");
    }

    // Wraps a working transport and makes exactly one chosen ReadAsync call hang forever instead
    // of returning, the same way a torn-down Linux SerialPort's pending read never completes on
    // its own -- only disposing it unsticks the hang, at which point every later call fails fast
    // instead of ever working again, matching a real dead serial port.
    private class HangOnceOnReadTransport(ISerialTransport inner, int hangOnReadCall) : ISerialTransport
    {
        private readonly TaskCompletionSource<int> _readHang = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _readCallCount;
        private bool _disposed;

        public Task OpenAsync(CancellationToken token) => inner.OpenAsync(token);

        public Task CloseAsync(CancellationToken token) => inner.CloseAsync(token);

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(HangOnceOnReadTransport), "The port is closed.");
            return inner.WriteAsync(buffer, token);
        }

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(HangOnceOnReadTransport), "The port is closed.");

            _readCallCount++;
            return _readCallCount == hangOnReadCall ? _readHang.Task : inner.ReadAsync(buffer, token);
        }

        public void Dispose()
        {
            _disposed = true;
            _readHang.TrySetException(
                new ObjectDisposedException(nameof(HangOnceOnReadTransport), "The port is closed.")
            );
            inner.Dispose();
        }
    }

    // Wraps a working transport and makes exactly one chosen ReadAsync call throw IOException,
    // the way .NET's SerialPort is documented to on Linux when the underlying device disappears
    // (as opposed to HangOnceOnReadTransport's silent-hang failure mode above).
    private class ThrowOnceOnReadTransport(ISerialTransport inner, int throwOnReadCall) : ISerialTransport
    {
        private int _readCallCount;

        public Task OpenAsync(CancellationToken token) => inner.OpenAsync(token);

        public Task CloseAsync(CancellationToken token) => inner.CloseAsync(token);

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) => inner.WriteAsync(buffer, token);

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            _readCallCount++;
            if (_readCallCount == throwOnReadCall)
                throw new IOException("Input/output error");
            return inner.ReadAsync(buffer, token);
        }

        public void Dispose() => inner.Dispose();
    }
}
