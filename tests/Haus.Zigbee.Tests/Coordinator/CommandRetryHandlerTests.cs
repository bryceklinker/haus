using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class CommandRetryHandlerTests
{
    private const byte ApsSuccess = 0x00;
    private const byte ApsNoAck = 0xa7;
    private const byte ApsNoBoundDevice = 0xa8;

    [Fact]
    public async Task WhenConfirmIsSuccess_ReturnsImmediately()
    {
        var attempts = new List<int>();
        var handler = new CommandRetryHandler(new CommandRetryOptions());

        var result = await handler.ExecuteWithRetryAsync(
            attempt =>
            {
                attempts.Add(attempt);
                return Task.FromResult(MakeConfirm(ApsSuccess));
            },
            CancellationToken.None
        );

        Assert.Single(attempts);
        Assert.Equal(ApsSuccess, result.ConfirmStatus);
    }

    [Fact]
    public async Task WhenConfirmFails_RetriesUpToMaxRetries()
    {
        var attempts = new List<int>();
        var options = new CommandRetryOptions { MaxRetries = 3 };
        var handler = new CommandRetryHandler(options);

        var ex = await Assert.ThrowsAsync<CommandDeliveryFailedException>(() =>
            handler.ExecuteWithRetryAsync(
                attempt =>
                {
                    attempts.Add(attempt);
                    return Task.FromResult(MakeConfirm(ApsNoAck));
                },
                CancellationToken.None
            )
        );

        Assert.Equal(4, attempts.Count);
        Assert.Equal(ApsNoAck, ex.LastConfirmStatus);
        Assert.Equal(4, ex.AttemptCount);
    }

    [Fact]
    public async Task WhenRetrySucceeds_ReturnsSuccessfulConfirm()
    {
        var attemptCount = 0;
        var options = new CommandRetryOptions { MaxRetries = 3 };
        var handler = new CommandRetryHandler(options);

        var result = await handler.ExecuteWithRetryAsync(
            attempt =>
            {
                attemptCount++;
                var status = attemptCount < 3 ? ApsNoAck : ApsSuccess;
                return Task.FromResult(MakeConfirm(status));
            },
            CancellationToken.None
        );

        Assert.Equal(3, attemptCount);
        Assert.Equal(ApsSuccess, result.ConfirmStatus);
    }

    [Fact]
    public async Task BackoffIncreasesExponentially()
    {
        var delays = new List<TimeSpan>();
        var options = new CommandRetryOptions
        {
            MaxRetries = 4,
            BaseBackoffMs = 100,
            MaxBackoffMs = 10000,
        };
        var handler = new CommandRetryHandler(
            options,
            delay =>
            {
                delays.Add(delay);
                return Task.CompletedTask;
            }
        );

        await Assert.ThrowsAsync<CommandDeliveryFailedException>(() =>
            handler.ExecuteWithRetryAsync(_ => Task.FromResult(MakeConfirm(ApsNoAck)), CancellationToken.None)
        );

        Assert.Equal(4, delays.Count);
        Assert.True(delays[0].TotalMilliseconds >= 50 && delays[0].TotalMilliseconds <= 150);
        Assert.True(delays[1].TotalMilliseconds >= 100 && delays[1].TotalMilliseconds <= 300);
        Assert.True(delays[2].TotalMilliseconds >= 200 && delays[2].TotalMilliseconds <= 600);
        Assert.True(delays[3].TotalMilliseconds >= 400 && delays[3].TotalMilliseconds <= 1200);
    }

    [Fact]
    public async Task BackoffIsCappedAtMaxBackoff()
    {
        var delays = new List<TimeSpan>();
        var options = new CommandRetryOptions
        {
            MaxRetries = 10,
            BaseBackoffMs = 1000,
            MaxBackoffMs = 2000,
        };
        var handler = new CommandRetryHandler(
            options,
            delay =>
            {
                delays.Add(delay);
                return Task.CompletedTask;
            }
        );

        await Assert.ThrowsAsync<CommandDeliveryFailedException>(() =>
            handler.ExecuteWithRetryAsync(_ => Task.FromResult(MakeConfirm(ApsNoAck)), CancellationToken.None)
        );

        foreach (var delay in delays)
        {
            Assert.True(delay.TotalMilliseconds <= 2500, $"Delay {delay.TotalMilliseconds}ms exceeded max with jitter");
        }
    }

    [Fact]
    public async Task WhenCancelled_DoesNotRetry()
    {
        var attemptCount = 0;
        var options = new CommandRetryOptions { MaxRetries = 3 };
        using var cts = new CancellationTokenSource();
        var handler = new CommandRetryHandler(options);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            handler.ExecuteWithRetryAsync(
                async _ =>
                {
                    attemptCount++;
                    cts.Cancel();
                    await Task.Delay(1, cts.Token);
                    return MakeConfirm(ApsNoAck);
                },
                cts.Token
            )
        );

        Assert.Equal(1, attemptCount);
    }

    private static ApsDataConfirm MakeConfirm(byte status)
    {
        return new ApsDataConfirm(
            SequenceNumber: 0,
            DeviceState: 0,
            RequestId: 0,
            DestinationAddressMode: DeconzAddressMode.Nwk,
            DestinationShortAddress: 0x1234,
            DestinationIeeeAddress: null,
            DestinationEndpoint: 1,
            SourceEndpoint: 1,
            ConfirmStatus: status
        );
    }
}
