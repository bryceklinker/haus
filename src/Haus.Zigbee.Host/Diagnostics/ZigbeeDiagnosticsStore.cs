using System;

namespace Haus.Zigbee.Host.Diagnostics;

public interface IZigbeeDiagnosticsStore
{
    ZigbeeDiagnosticsState Current { get; }

    void Update(Func<ZigbeeDiagnosticsState, ZigbeeDiagnosticsState> transform);
}

public class ZigbeeDiagnosticsStore : IZigbeeDiagnosticsStore
{
    private readonly object _gate = new();
    private ZigbeeDiagnosticsState _current = ZigbeeDiagnosticsState.Initial;

    public ZigbeeDiagnosticsState Current
    {
        get
        {
            lock (_gate)
                return _current;
        }
    }

    public void Update(Func<ZigbeeDiagnosticsState, ZigbeeDiagnosticsState> transform)
    {
        lock (_gate)
            _current = transform(_current);
    }
}
