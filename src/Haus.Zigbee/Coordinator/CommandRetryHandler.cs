using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Coordinator;

public class CommandRetryHandler
{
    private const byte ApsSuccess = 0x00;
    private static readonly Random Jitter = new();

    private readonly CommandRetryOptions _options;
    private readonly Func<TimeSpan, Task> _delayFunc;

    public CommandRetryHandler(CommandRetryOptions options)
        : this(options, delay => Task.Delay(delay)) { }

    public CommandRetryHandler(CommandRetryOptions options, Func<TimeSpan, Task> delayFunc)
    {
        _options = options;
        _delayFunc = delayFunc;
    }

    public async Task<ApsDataConfirm> ExecuteWithRetryAsync(
        Func<int, Task<ApsDataConfirm>> operation,
        CancellationToken token
    )
    {
        var attempt = 0;
        ApsDataConfirm? lastConfirm = null;

        while (attempt <= _options.MaxRetries)
        {
            token.ThrowIfCancellationRequested();

            lastConfirm = await operation(attempt);

            if (lastConfirm.ConfirmStatus == ApsSuccess)
                return lastConfirm;

            if (attempt < _options.MaxRetries)
            {
                var delay = CalculateBackoff(attempt);
                await _delayFunc(delay);
            }

            attempt++;
        }

        throw new CommandDeliveryFailedException(lastConfirm!.ConfirmStatus, attempt);
    }

    private TimeSpan CalculateBackoff(int attempt)
    {
        var baseDelayMs = _options.BaseBackoffMs * Math.Pow(2, attempt);
        var jitterMs = (Jitter.NextDouble() - 0.5) * _options.BaseBackoffMs;
        var totalMs = Math.Min(baseDelayMs + jitterMs, _options.MaxBackoffMs);
        return TimeSpan.FromMilliseconds(Math.Max(0, totalMs));
    }
}
