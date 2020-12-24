using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Haus.Device.Simulator.Devices.Services
{
    public interface IDevicesStore
    {
        IDevicesState Current { get; }
        IDisposable Subscribe(Action<IDevicesState> handler);
        void Next(IDevicesState state);
        IDisposable SubscribeAsync(Func<IDevicesState, Task> handler);
    }
    
    public class DevicesStore : IDevicesStore
    {
        private readonly BehaviorSubject<IDevicesState> _subject = new(new DevicesState());
        
        public IDevicesState Current => _subject.Value;
        
        public IDisposable Subscribe(Action<IDevicesState> handler)
        {
            return _subject.SubscribeSafe(Observer.Create(handler));
        }

        public IDisposable SubscribeAsync(Func<IDevicesState, Task> handler)
        {
            return _subject.SelectMany(state => Observable.FromAsync(async () =>
            {
                await handler(state).ConfigureAwait(false);
                return Unit.Default;
            })).Subscribe();
        }

        public void Next(IDevicesState state)
        {
            _subject.OnNext(state);
        }
    }
}