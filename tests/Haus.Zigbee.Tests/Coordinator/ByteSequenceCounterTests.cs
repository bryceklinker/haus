using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Haus.Zigbee.Coordinator;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class ByteSequenceCounterTests
{
    [Fact]
    public void Next_CalledConcurrentlyFromManyThreads_ReturnsEveryValueInOneWraparoundExactlyOnce()
    {
        // Real OS threads, not the thread pool: 256 threads blocked on a Barrier can starve the
        // pool's throttled thread-injection, which doesn't deadlock but makes the test hang for
        // minutes waiting for threads to ramp up.
        const int callCount = 256;
        var counter = new ByteSequenceCounter();
        var results = new ConcurrentBag<byte>();
        using var barrier = new Barrier(callCount);

        var threads = Enumerable
            .Range(0, callCount)
            .Select(_ => new Thread(() =>
            {
                barrier.SignalAndWait();
                results.Add(counter.Next());
            }))
            .ToList();
        foreach (var thread in threads)
            thread.Start();
        foreach (var thread in threads)
            thread.Join();

        Assert.Equal(callCount, results.Distinct().Count());
    }
}
