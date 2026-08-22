using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial.Frames;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
    private readonly ILogger<ApsSender> _logger;
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<ApsDataConfirm>> _pendingConfirms = new();

    // Interviews for different devices call SendAsync concurrently (each is its own detached
    // task), and this increment happens before SendAndReceiveAsync's mutex is even acquired, so it
    // needs its own atomic counter rather than a plain field.
    private readonly ByteSequenceCounter _sequenceNumber = new();

    public ApsSender(
        ApsPollLoop pollLoop,
        DeconzChannel channel,
        TimeSpan? confirmTimeout = null,
        ILogger<ApsSender>? logger = null
    )
    {
        _pollLoop = pollLoop;
        _channel = channel;
        _confirmTimeout = confirmTimeout ?? DefaultConfirmTimeout;
        _logger = logger ?? NullLogger<ApsSender>.Instance;
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
            var command = ApsDataRequestFrameCodec.Encode(request with { SequenceNumber = _sequenceNumber.Next() });
            await _channel.SendAndReceiveAsync(command, token);

            using var timeout = new CancellationTokenSource(_confirmTimeout);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, timeout.Token);
            try
            {
                var confirm = await pendingConfirm.Task.WaitAsync(linked.Token);
                _logger.LogDebug("Received confirm for request 0x{@RequestId:x2}", request.RequestId);
                return confirm;
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "Timed out after {@Timeout} waiting for confirm for request 0x{@RequestId:x2}",
                    _confirmTimeout,
                    request.RequestId
                );
                throw;
            }
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
