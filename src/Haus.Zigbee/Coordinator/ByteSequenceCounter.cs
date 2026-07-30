using System.Threading;

namespace Haus.Zigbee.Coordinator;

// A wraparound byte counter that DeviceInterview uses for its transaction-sequence-number and
// request-id fields. Interviews for different devices run as independently scheduled tasks and
// can call Next() concurrently, so a plain field increment (read, add, write as separate steps)
// can lose updates under real thread contention.
public class ByteSequenceCounter
{
    private int _value;

    public byte Next()
    {
        return (byte)(Interlocked.Increment(ref _value) - 1);
    }
}
