using System;

namespace Haus.Core.Common
{
    public interface IClock
    {
        DateTime UtcNow { get; }
        DateTime LocalNow { get; }
    }
    
    public class Clock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime LocalNow => UtcNow.ToLocalTime();
    }
}