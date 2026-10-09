using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class SleepyCommandHoldTests
{
    private const byte SuccessStatus = 0x00;

    [Fact]
    public void DefaultTtlIsOneDay()
    {
        var options = new SleepyHoldOptions();

        Assert.Equal(TimeSpan.FromDays(1), options.Ttl);
    }

    [Fact]
    public async Task WhenReleasedBeforeTtlExpiresThenTheOperationRunsAndItsResultIsReturned()
    {
        var hold = new SleepyCommandHold(new SleepyHoldOptions(), _ => Task.Delay(Timeout.Infinite));
        var key = DeviceKey.FromIeee(new IeeeAddress(1));
        var expected = AnyConfirm();

        var holdTask = hold.HoldAsync(key, _ => Task.FromResult(expected), CancellationToken.None);
        hold.Release(key, CancellationToken.None);

        var result = await holdTask;
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task WhenTtlElapsesWithoutReleaseThenTheHeldTaskFaultsWithCommandHoldExpiredException()
    {
        var hold = new SleepyCommandHold(new SleepyHoldOptions(), _ => Task.CompletedTask);
        var key = DeviceKey.FromIeee(new IeeeAddress(1));

        var holdTask = hold.HoldAsync(key, _ => Task.FromResult(AnyConfirm()), CancellationToken.None);

        await Assert.ThrowsAsync<CommandHoldExpiredException>(() => holdTask);
    }

    [Fact]
    public async Task WhenReleasedAfterItAlreadyExpiredThenTheOperationIsNotRun()
    {
        var hold = new SleepyCommandHold(new SleepyHoldOptions(), _ => Task.CompletedTask);
        var key = DeviceKey.FromIeee(new IeeeAddress(1));
        var operationRan = false;

        var holdTask = hold.HoldAsync(
            key,
            _ =>
            {
                operationRan = true;
                return Task.FromResult(AnyConfirm());
            },
            CancellationToken.None
        );
        await Assert.ThrowsAsync<CommandHoldExpiredException>(() => holdTask);

        hold.Release(key, CancellationToken.None);

        Assert.False(operationRan);
    }

    [Fact]
    public async Task WhenASecondCommandIsHeldForTheSameDeviceThenTheFirstHoldIsSuperseded()
    {
        var hold = new SleepyCommandHold(new SleepyHoldOptions(), _ => Task.Delay(Timeout.Infinite));
        var key = DeviceKey.FromIeee(new IeeeAddress(1));

        var firstHold = hold.HoldAsync(key, _ => Task.FromResult(AnyConfirm()), CancellationToken.None);
        var secondHold = hold.HoldAsync(key, _ => Task.FromResult(AnyConfirm()), CancellationToken.None);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => firstHold);

        hold.Release(key, CancellationToken.None);
        await secondHold;
    }

    [Fact]
    public async Task WhenReleasedForADeviceWithNoHeldCommandThenNothingHappens()
    {
        var hold = new SleepyCommandHold(new SleepyHoldOptions(), _ => Task.Delay(Timeout.Infinite));
        var key = DeviceKey.FromIeee(new IeeeAddress(1));

        hold.Release(key, CancellationToken.None);

        await Task.CompletedTask;
    }

    private static ApsDataConfirm AnyConfirm()
    {
        return new ApsDataConfirm(
            SequenceNumber: 0x00,
            DeviceState: 0x00,
            RequestId: 0x00,
            DestinationAddressMode: DeconzAddressMode.Nwk,
            DestinationShortAddress: 0x1234,
            DestinationIeeeAddress: null,
            DestinationEndpoint: 0x01,
            SourceEndpoint: 0x01,
            ConfirmStatus: SuccessStatus
        );
    }
}
