using System;

namespace Haus.Zigbee.Host.Tests.Support;

public class OptionsListenerFake<T>(Action<T, string> handler, Action<OptionsListenerFake<T>> onDispose)
    : IDisposable
{
    public void Trigger(T value, string name)
    {
        handler.Invoke(value, name);
    }

    public void Dispose()
    {
        onDispose.Invoke(this);
    }
}