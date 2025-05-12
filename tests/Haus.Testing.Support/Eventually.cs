using System;
using System.Threading.Tasks;
using Polly;

namespace Haus.Testing.Support;

public class EventuallyException(Exception innerException, double timeout)
    : Exception($"Eventually failed after {timeout} ms.", innerException);

public static class Eventually
{
    private const int DefaultTimeout = 5000;
    private const int DefaultDelay = 100;

    public static void Assert(Action assertion, double timeout = DefaultTimeout, int delay = DefaultDelay)
    {
        try
        {
            Policy.Handle<Exception>().WaitAndRetry(DelayGenerator.Generate(timeout, delay)).Execute(assertion);
        }
        catch (Exception e)
        {
            HandleException(e, timeout);
        }
    }

    public static async Task AssertAsync(
        Func<Task> assertion,
        double timeout = DefaultTimeout,
        int delay = DefaultDelay
    )
    {
        try
        {
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(DelayGenerator.Generate(timeout, delay))
                .ExecuteAsync(assertion);
        }
        catch (Exception e)
        {
            HandleException(e, timeout);
        }
    }

    private static void HandleException(Exception exception, double timeout)
    {
        throw new EventuallyException(exception, timeout);
    }
}
