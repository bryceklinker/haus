using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class DeviceCommandQueueTests
{
    [Fact]
    public async Task CommandsToSameDevice_AreSerializedInOrder()
    {
        var executionOrder = new List<int>();
        var device = new IeeeAddress(0x0011223344556677);
        var queue = new DeviceCommandQueue();

        var tcs1 = new TaskCompletionSource<int>();
        var tcs2 = new TaskCompletionSource<int>();
        var tcs3 = new TaskCompletionSource<int>();

        var task1 = queue.EnqueueAsync(
            device,
            async _ =>
            {
                executionOrder.Add(1);
                return await tcs1.Task;
            },
            CancellationToken.None
        );

        var task2 = queue.EnqueueAsync(
            device,
            async _ =>
            {
                executionOrder.Add(2);
                return await tcs2.Task;
            },
            CancellationToken.None
        );

        var task3 = queue.EnqueueAsync(
            device,
            async _ =>
            {
                executionOrder.Add(3);
                return await tcs3.Task;
            },
            CancellationToken.None
        );

        await Task.Delay(50);
        Assert.Single(executionOrder);
        Assert.Equal(1, executionOrder[0]);

        tcs1.SetResult(100);
        await task1;

        await Task.Delay(50);
        Assert.Equal(2, executionOrder.Count);
        Assert.Equal(2, executionOrder[1]);

        tcs2.SetResult(200);
        await task2;

        await Task.Delay(50);
        Assert.Equal(3, executionOrder.Count);
        Assert.Equal(3, executionOrder[2]);

        tcs3.SetResult(300);
        var result3 = await task3;
        Assert.Equal(300, result3);
    }

    [Fact]
    public async Task CommandsToDifferentDevices_ExecuteInParallel()
    {
        var executionStartTimes = new ConcurrentDictionary<IeeeAddress, DateTime>();
        var device1 = new IeeeAddress(0x1111111111111111);
        var device2 = new IeeeAddress(0x2222222222222222);
        var device3 = new IeeeAddress(0x3333333333333333);
        var queue = new DeviceCommandQueue();

        var allStarted = new TaskCompletionSource();
        var canComplete = new TaskCompletionSource();

        async Task<int> ExecuteCommand(IeeeAddress device, CancellationToken token)
        {
            executionStartTimes[device] = DateTime.UtcNow;
            if (executionStartTimes.Count == 3)
                allStarted.TrySetResult();
            await canComplete.Task;
            return 1;
        }

        var task1 = queue.EnqueueAsync(device1, token => ExecuteCommand(device1, token), CancellationToken.None);
        var task2 = queue.EnqueueAsync(device2, token => ExecuteCommand(device2, token), CancellationToken.None);
        var task3 = queue.EnqueueAsync(device3, token => ExecuteCommand(device3, token), CancellationToken.None);

        var startedInTime = await Task.WhenAny(allStarted.Task, Task.Delay(1000)) == allStarted.Task;
        Assert.True(startedInTime, "All three commands should start in parallel");

        canComplete.SetResult();
        await Task.WhenAll(task1, task2, task3);
    }

    [Fact]
    public async Task WhenCommandThrows_NextCommandStillExecutes()
    {
        var device = new IeeeAddress(0x1234567890abcdef);
        var queue = new DeviceCommandQueue();
        var secondExecuted = false;

        var task1 = queue.EnqueueAsync<int>(
            device,
            _ => throw new InvalidOperationException("Test failure"),
            CancellationToken.None
        );
        var task2 = queue.EnqueueAsync(
            device,
            _ =>
            {
                secondExecuted = true;
                return Task.FromResult(42);
            },
            CancellationToken.None
        );

        await Assert.ThrowsAsync<InvalidOperationException>(() => task1);
        var result2 = await task2;

        Assert.True(secondExecuted);
        Assert.Equal(42, result2);
    }

    [Fact]
    public async Task WhenCancelled_PropagatesCancellation()
    {
        var device = new IeeeAddress(0xaabbccddeeff0011);
        var queue = new DeviceCommandQueue();
        using var cts = new CancellationTokenSource();

        var neverCompletes = new TaskCompletionSource<int>();
        var task1 = queue.EnqueueAsync(
            device,
            async token =>
            {
                token.Register(() => neverCompletes.TrySetCanceled(token));
                return await neverCompletes.Task;
            },
            cts.Token
        );

        await Task.Delay(20);
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task1);
    }
}
