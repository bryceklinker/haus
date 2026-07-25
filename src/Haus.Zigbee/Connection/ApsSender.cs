using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

// Sends an APS data-request over the deCONZ channel and completes when the coordinator later
// reports the matching delivery confirm. The deCONZ-level round trip only tells us the send was
// queued; the real delivery outcome arrives asynchronously through the poll loop's confirm event,
// so this correlates each outstanding request to its confirm by RequestId.
public sealed class ApsSender
{
    private readonly DeconzChannel _channel;
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<ApsDataConfirm>> _pendingConfirms = new();
    private byte _sequenceNumber;

    public ApsSender(ApsPollLoop pollLoop, DeconzChannel channel)
    {
        _channel = channel;
        pollLoop.ConfirmReceived += OnConfirmReceived;
    }

    public async Task<ApsDataConfirm> SendAsync(ApsDataRequestFrame request, CancellationToken token)
    {
        var pendingConfirm = new TaskCompletionSource<ApsDataConfirm>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        _pendingConfirms[request.RequestId] = pendingConfirm;

        var command = ApsDataRequestFrameCodec.Encode(request with { SequenceNumber = _sequenceNumber++ });
        await _channel.SendAndReceiveAsync(command, token);

        return await pendingConfirm.Task;
    }

    private void OnConfirmReceived(object? sender, ApsDataConfirm confirm)
    {
        if (_pendingConfirms.TryRemove(confirm.RequestId, out var pendingConfirm))
            pendingConfirm.SetResult(confirm);
    }
}
