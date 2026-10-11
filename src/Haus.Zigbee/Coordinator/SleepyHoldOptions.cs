using System;

namespace Haus.Zigbee.Coordinator;

public class SleepyHoldOptions
{
    // Matches zigbee-herdsman's pendingRequestTimeout fallback (one day) for a device whose
    // checkin interval isn't known -- Haus has no Poll Control cluster handling and so no learned
    // interval to use instead, per the design note's locked TTL decision.
    public TimeSpan Ttl { get; set; } = TimeSpan.FromDays(1);
}
