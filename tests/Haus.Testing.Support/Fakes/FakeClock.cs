using System;
using Haus.Core.Common;

namespace Haus.Testing.Support.Fakes
{
    public class FakeClock : IClock
    {
        public DateTime UtcNow { get; private set; } = DateTime.UtcNow;
        public DateTime LocalNow => UtcNow.ToLocalTime();

        public void SetNow(DateTime time)
        {
            UtcNow = time.ToUniversalTime();
        }
    }
}