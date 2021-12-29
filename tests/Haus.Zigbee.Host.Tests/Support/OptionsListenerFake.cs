using System;

namespace Haus.Zigbee.Host.Tests.Support;

public class OptionsListenerFake<T> : IDisposable
{
    private readonly Action<T, string> _handler;
    private readonly Action<OptionsListenerFake<T>> _onDispose;

    public OptionsListenerFake(Action<T, string> handler, Action<OptionsListenerFake<T>> onDispose)
    {
        _handler = handler;
        _onDispose = onDispose;
    }

    public void Trigger(T value, string name)
    {
        _handler.Invoke(value, name);
    }

    public void Dispose()
    {
        _onDispose.Invoke(this);
    }
}