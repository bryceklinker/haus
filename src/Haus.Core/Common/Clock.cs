using System;

namespace Haus.Core.Common;

public interface IClock
{
    DateTime UtcNow { get; }
    DateTime LocalNow { get; }
    DateTimeOffset UtcNowOffset { get; }
    DateTimeOffset LocalNowOffset { get; }
}

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime LocalNow => UtcNow.ToLocalTime();
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    public DateTimeOffset LocalNowOffset => UtcNowOffset.ToLocalTime();
}