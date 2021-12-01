using System;
using System.Threading.Tasks;
using Polly;

namespace Haus.Testing.Support
{
    public static class Eventually
    {
        private const int DefaultTimeout = 5000;
        private const int DefaultDelay = 100;

        public static void Assert(Action assertion, double timeout = DefaultTimeout, int delay = DefaultDelay)
        {
            Policy.Handle<Exception>()
                .WaitAndRetry(DelayGenerator.Generate(timeout, delay))
                .Execute(assertion);
        }
        
        public static async Task AssertAsync(Func<Task> assertion, double timeout = DefaultTimeout, int delay = DefaultDelay)
        {
            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(DelayGenerator.Generate(timeout, delay))
                .ExecuteAsync(assertion);
        }
    }
}