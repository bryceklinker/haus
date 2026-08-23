using System;
using System.Reactive.Subjects;

namespace Haus.Core.Zigbee.State;

public interface IZigbeeStore : IObservable<IZigbeeState>
{
    IZigbeeState Current { get; }
    void Publish(IZigbeeState state);
    void PublishNext(Func<IZigbeeState, IZigbeeState> generateNextState);
}

public class ZigbeeStore : IZigbeeStore
{
    private readonly BehaviorSubject<IZigbeeState> _subject = new(ZigbeeState.Initial);

    public IZigbeeState Current => _subject.Value;

    public void Publish(IZigbeeState state)
    {
        _subject.OnNext(state);
    }

    public void PublishNext(Func<IZigbeeState, IZigbeeState> generateNextState)
    {
        var next = generateNextState(Current);
        if (ReferenceEquals(next, Current))
            return;

        Publish(next);
    }

    public IDisposable Subscribe(IObserver<IZigbeeState> observer)
    {
        return _subject.Subscribe(observer);
    }
}
