using System;
using System.Threading.Tasks;
using Polly;

namespace Haus.Testing.Support
{
    public static class WaitFor
    {
        private const int DefaultTimeout = 5000;
        private const int DefaultDelay = 100;
        
        public static async Task<T> ResultAsync<T>(Func<Task<T>> executor, int timeout = DefaultTimeout, int delay = DefaultDelay)
        {
            var delays = DelayGenerator.Generate(timeout, delay);
            return await Policy.Handle<Exception>()
                .WaitAndRetryAsync(delays)
                .ExecuteAsync(executor);
        }
    }
}