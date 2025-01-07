using System;
using System.Reactive.Subjects;
using Haus.Core.DeviceSimulator.Entities;

namespace Haus.Core.DeviceSimulator.State;

public interface IDeviceSimulatorStore : IObservable<IDeviceSimulatorState>
{
    IDeviceSimulatorState Current { get; }

    SimulatedDeviceEntity GetDeviceById(string deviceId);
    void Publish(IDeviceSimulatorState state);
    void PublishNext(Func<IDeviceSimulatorState, IDeviceSimulatorState> generateNextState);
}

public class DeviceSimulatorStore : IDeviceSimulatorStore
{
    private readonly BehaviorSubject<IDeviceSimulatorState> _subject = new(DeviceSimulatorState.Initial);

    public IDeviceSimulatorState Current => _subject.Value;

    public SimulatedDeviceEntity GetDeviceById(string deviceId)
    {
        return Current.GetDeviceById(deviceId);
    }

    public void Publish(IDeviceSimulatorState state)
    {
        _subject.OnNext(state);
    }

    public void PublishNext(Func<IDeviceSimulatorState, IDeviceSimulatorState> generateNextState)
    {
        var next = generateNextState(Current);
        if (ReferenceEquals(next, Current))
            return;

        Publish(next);
    }

    public IDisposable Subscribe(IObserver<IDeviceSimulatorState> observer)
    {
        return _subject.Subscribe(observer);
    }
}