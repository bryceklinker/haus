using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;
using Polly;
using Polly.Retry;

namespace Haus.Zigbee.Coordinator;

public class CommandRetryHandler
{
    private const byte ApsSuccess = 0x00;
    private static readonly Random Jitter = new();

    private readonly CommandRetryOptions _options;
    private readonly Func<TimeSpan, Task> _delayFunc;
    private readonly ResiliencePipeline<ApsDataConfirm> _pipeline;

    public CommandRetryHandler(CommandRetryOptions options)
        : this(options, delay => Task.Delay(delay)) { }

    public CommandRetryHandler(CommandRetryOptions options, Func<TimeSpan, Task> delayFunc)
    {
        _options = options;
        _delayFunc = delayFunc;
        _pipeline = BuildPipeline();
    }

    public async Task<ApsDataConfirm> ExecuteWithRetryAsync(
        Func<int, Task<ApsDataConfirm>> operation,
        CancellationToken token
    )
    {
        var attemptCount = 0;

        var result = await _pipeline.ExecuteAsync(
            async ct =>
            {
                ct.ThrowIfCancellationRequested();
                var confirm = await operation(attemptCount);
                attemptCount++;
                return confirm;
            },
            token
        );

        if (result.ConfirmStatus != ApsSuccess)
            throw new CommandDeliveryFailedException(result.ConfirmStatus, attemptCount);

        return result;
    }

    private ResiliencePipeline<ApsDataConfirm> BuildPipeline()
    {
        // RetryStrategyOptions requires MaxRetryAttempts >= 1; MaxRetries == 0 means "attempt once,
        // never retry", which the empty pipeline already gives us without an initial failing attempt.
        if (_options.MaxRetries <= 0)
            return ResiliencePipeline<ApsDataConfirm>.Empty;

        return new ResiliencePipelineBuilder<ApsDataConfirm>()
            .AddRetry(
                new RetryStrategyOptions<ApsDataConfirm>
                {
                    ShouldHandle = new PredicateBuilder<ApsDataConfirm>().HandleResult(confirm =>
                        confirm.ConfirmStatus != ApsSuccess
                    ),
                    MaxRetryAttempts = _options.MaxRetries,
                    // Backoff timing is computed and awaited by hand below via OnRetry so tests can
                    // inject a synchronous delayFunc stand-in and cancellation stays responsive;
                    // Polly's own delay is disabled here to avoid a second, redundant sleep.
                    Delay = TimeSpan.Zero,
                    OnRetry = async args =>
                    {
                        var delay = CalculateBackoff(args.AttemptNumber);
                        await WaitCancellableAsync(delay, args.Context.CancellationToken);
                    },
                }
            )
            .Build();
    }

    private async Task WaitCancellableAsync(TimeSpan delay, CancellationToken token)
    {
        // _delayFunc doesn't accept a token (tests inject a synchronous stand-in to record/skip
        // delays), so race it against the caller's token here to keep backoff waits responsive to
        // cancellation instead of blocking for the full delay before the pipeline retries.
        var delayTask = _delayFunc(delay);
        var cancellationTask = Task.Delay(Timeout.InfiniteTimeSpan, token);
        var completed = await Task.WhenAny(delayTask, cancellationTask);
        if (completed == cancellationTask)
            token.ThrowIfCancellationRequested();
    }

    private TimeSpan CalculateBackoff(int attempt)
    {
        var baseDelayMs = _options.BaseBackoffMs * Math.Pow(2, attempt);
        var jitterMs = (Jitter.NextDouble() - 0.5) * _options.BaseBackoffMs;
        var totalMs = Math.Min(baseDelayMs + jitterMs, _options.MaxBackoffMs);
        return TimeSpan.FromMilliseconds(Math.Max(0, totalMs));
    }
}
