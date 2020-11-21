using System;
using System.Threading.Tasks;

namespace Haus.Testing.Support
{
    public static class Eventually
    {
        private const int DefaultTimeout = 5000;
        private const int DefaultDelay = 100;

        public static async Task Assert(Action assertion, int timeout = DefaultTimeout, int delay = DefaultDelay)
        {
            Exception assertionException = null;
            var endTime = DateTime.UtcNow.AddMilliseconds(timeout);
            while (endTime >= DateTime.UtcNow)
            {
                try
                {
                    assertion();
                    return;
                }
                catch (Exception exception)
                {
                    assertionException = exception;
                    await Task.Delay(delay);
                }
            }

            if (assertionException == null)
                return;

            throw assertionException;
        }
        
        public static async Task AssertAsync(Func<Task> assertion, int timeout = DefaultTimeout, int delay = DefaultDelay)
        {
            Exception assertionException = null;
            var endTime = DateTime.UtcNow.AddMilliseconds(timeout);
            while (endTime >= DateTime.UtcNow)
            {
                try
                {
                    await assertion();
                    return;
                }
                catch (Exception exception)
                {
                    assertionException = exception;
                    await Task.Delay(delay);
                }
            }

            if (assertionException == null)
                return;

            throw assertionException;
        }
    }
}