using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Haus.Core;

public static class ObservableExtensions
{
    public static IDisposable SubscribeAsync<T>(this IObservable<T> source, Func<T, Task> observer)
    {
        return source
            .SelectMany(async value =>
            {
                await observer(value).ConfigureAwait(false);
                return Unit.Default;
            })
            .Subscribe();
    }
}
