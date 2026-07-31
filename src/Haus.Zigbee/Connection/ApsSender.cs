using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

// Sends an APS data-request over the deCONZ channel and completes when the coordinator later
// reports the matching delivery confirm. The deCONZ-level round trip only tells us the send was
// queued; the real delivery outcome arrives asynchronously through the poll loop's confirm event,
// so this correlates each outstanding request to its confirm by RequestId.
public class ApsSender : IDisposable
{
    private static readonly TimeSpan DefaultConfirmTimeout = TimeSpan.FromSeconds(30);

    private readonly ApsPollLoop _pollLoop;
    private readonly DeconzChannel _channel;
    private readonly TimeSpan _confirmTimeout;
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<ApsDataConfirm>> _pendingConfirms = new();
    private byte _sequenceNumber;

    public ApsSender(ApsPollLoop pollLoop, DeconzChannel channel, TimeSpan? confirmTimeout = null)
    {
        _pollLoop = pollLoop;
        _channel = channel;
        _confirmTimeout = confirmTimeout ?? DefaultConfirmTimeout;
        _pollLoop.ConfirmReceived += OnConfirmReceived;
    }

    public void Dispose()
    {
        _pollLoop.ConfirmReceived -= OnConfirmReceived;
        GC.SuppressFinalize(this);
    }

    // A device that never confirms delivery would otherwise hang this forever and leak its entry
    // in _pendingConfirms permanently, since nothing else ever removes it.
    public async Task<ApsDataConfirm> SendAsync(ApsDataRequestFrame request, CancellationToken token)
    {
        var pendingConfirm = new TaskCompletionSource<ApsDataConfirm>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        _pendingConfirms[request.RequestId] = pendingConfirm;
        try
        {
            var command = ApsDataRequestFrameCodec.Encode(request with { SequenceNumber = _sequenceNumber++ });
            await _channel.SendAndReceiveAsync(command, token);

            using var timeout = new CancellationTokenSource(_confirmTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, timeout.Token);
            return await pendingConfirm.Task.WaitAsync(linked.Token);
        }
        finally
        {
            _pendingConfirms.TryRemove(request.RequestId, out _);
        }
    }

    private void OnConfirmReceived(object? sender, ApsDataConfirm confirm)
    {
        if (_pendingConfirms.TryRemove(confirm.RequestId, out var pendingConfirm))
            pendingConfirm.SetResult(confirm);
    }
}
