using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Tests.Support;

public class OptionsMonitorFake<T>(T value) : IOptionsMonitor<T>
{
    private readonly List<OptionsListenerFake<T>> _listeners = new();
    public T CurrentValue { get; private set; } = value;

    public T Get(string name)
    {
        return CurrentValue;
    }

    public IDisposable OnChange(Action<T, string> listener)
    {
        var handler = new OptionsListenerFake<T>(listener, obj => _listeners.Remove(obj));
        _listeners.Add(handler);
        return handler;
    }

    public void TriggerChange(T value, string name = null)
    {
        CurrentValue = value;
        foreach (var listener in _listeners)
            listener.Trigger(value, name);
    }
}
