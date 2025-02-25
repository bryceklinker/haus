using System;
using Haus.Core.Common;

namespace Haus.Testing.Support.Fakes;

public class FakeClock : IClock
{
    public DateTime UtcNow { get; private set; } = DateTime.UtcNow;
    public DateTime LocalNow => UtcNow.ToLocalTime();
    public DateTimeOffset UtcNowOffset { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LocalNowOffset => UtcNowOffset.ToLocalTime();

    public void SetNow(DateTime time)
    {
        SetNow(new DateTimeOffset(time.ToUniversalTime()));
    }

    public void SetNow(DateTimeOffset time)
    {
        UtcNow = time.DateTime;
        UtcNowOffset = time;
    }
}
