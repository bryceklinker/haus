using System;
using System.Threading.Tasks;

namespace Haus.Site.Host.Tests.Support.Realtime;

public class InMemoryRealtimeDataHandler(string eventName, Type inputType, Func<object?, Task> handler)
{
    public async Task InvokeAsync<T>(string @event, T data)
    {
        if (!ShouldInvoke<T>(@event))
            return;

        await handler.Invoke(data);
    }

    public static InMemoryRealtimeDataHandler Create<T>(string eventName, Func<T, Task> handler)
    {
        return new InMemoryRealtimeDataHandler(
            eventName,
            typeof(T),
            async value =>
            {
                if (value is T actual)
                    await handler(actual);
            }
        );
    }

    private bool ShouldInvoke<T>(string @event)
    {
        return @event == eventName && typeof(T) == inputType;
    }
}
