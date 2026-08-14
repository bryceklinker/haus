using System.Collections.Concurrent;

namespace Haus.Zigbee.Simulator;

// Pending indications/confirms a real coordinator would be holding for its next DeviceState
// poll -- the deCONZ protocol has the client drain these via separate ReadIndication/ReadConfirm
// commands rather than pushing them, so this just holds what's waiting to be drained.
public class DeconzMessageQueue
{
    private readonly ConcurrentQueue<IndicationBody> _indications = new();
    private readonly ConcurrentQueue<ConfirmBody> _confirms = new();

    public bool HasIndication => !_indications.IsEmpty;
    public bool HasConfirm => !_confirms.IsEmpty;

    public void EnqueueIndication(IndicationBody body)
    {
        _indications.Enqueue(body);
    }

    public void EnqueueConfirm(ConfirmBody body)
    {
        _confirms.Enqueue(body);
    }

    public bool TryDequeueIndication(out IndicationBody? body)
    {
        return _indications.TryDequeue(out body);
    }

    public bool TryDequeueConfirm(out ConfirmBody? body)
    {
        return _confirms.TryDequeue(out body);
    }
}
