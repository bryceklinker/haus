using System;
using System.Collections.Generic;

namespace Haus.Testing.Support
{
    public static class DelayGenerator
    {
        public static IEnumerable<TimeSpan> Generate(double timeout, int delay)
        {
            var endTime = DateTime.UtcNow.AddMilliseconds(timeout);
            while (endTime >= DateTime.UtcNow)
            {
                yield return TimeSpan.FromMilliseconds(delay);
            }
        }
    }
}