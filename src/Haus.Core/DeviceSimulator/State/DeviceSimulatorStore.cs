using System;
using System.Reactive.Subjects;

namespace Haus.Core.DeviceSimulator.State
{
    public interface IDeviceSimulatorStore : IObservable<IDeviceSimulatorState>
    {
        IDeviceSimulatorState Current { get; }
        void Publish(IDeviceSimulatorState state);
    }
    
    public class DeviceSimulatorStore : IDeviceSimulatorStore
    {
        private readonly BehaviorSubject<IDeviceSimulatorState> _subject;

        public IDeviceSimulatorState Current => _subject.Value;

        public DeviceSimulatorStore()
        {
            _subject = new BehaviorSubject<IDeviceSimulatorState>(DeviceSimulatorState.Initial);
        }

        public void Publish(IDeviceSimulatorState state)
        {
            _subject.OnNext(state);
        }

        public IDisposable Subscribe(IObserver<IDeviceSimulatorState> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}