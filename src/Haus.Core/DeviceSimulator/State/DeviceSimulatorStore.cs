using System;
using System.Reactive.Subjects;

namespace Haus.Core.DeviceSimulator.State
{
    public interface IDeviceSimulatorStore : IObservable<IDeviceSimulatorState>
    {
        IDeviceSimulatorState Current { get; }
        void Publish(IDeviceSimulatorState state);

        void PublishNext(Func<IDeviceSimulatorState, IDeviceSimulatorState> generateNextState);
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

        public void PublishNext(Func<IDeviceSimulatorState, IDeviceSimulatorState> generateNextState)
        {
            var next = generateNextState(Current);
            if (next == Current)
                return;
            
            Publish(next);
        }

        public IDisposable Subscribe(IObserver<IDeviceSimulatorState> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}