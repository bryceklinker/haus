using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial;
using Microsoft.Extensions.Logging;

namespace Haus.Zigbee.Simulator;

// Drives one accepted TCP connection: decodes SLIP frames off the socket, validates their
// checksum, hands the unframed request to the responder, then SLIP+checksum-frames whatever
// response it returns and writes it back. Mirrors what a real deCONZ dongle's serial line does,
// just carried over TCP instead of a physical port.
public class DongleConnection(TcpClient client, DeconzResponder responder, ILogger<DongleConnection> logger)
{
    private const int ReadBufferSize = 256;
    private const int ChecksumLength = 2;

    public async Task RunAsync(CancellationToken token)
    {
        using var _ = client;
        var stream = client.GetStream();
        var decoder = new SlipDecoder();
        var buffer = new byte[ReadBufferSize];

        try
        {
            while (!token.IsCancellationRequested)
            {
                var count = await stream.ReadAsync(buffer, token);
                if (count == 0)
                    return;

                foreach (var frame in decoder.Decode(buffer[..count]))
                    await RespondToFrameAsync(stream, frame, token);
            }
        }
        catch (Exception e) when (e is IOException or OperationCanceledException)
        {
            logger.LogInformation("Dongle connection closed: {@Message}", e.Message);
        }
    }

    private async Task RespondToFrameAsync(NetworkStream stream, byte[] frame, CancellationToken token)
    {
        if (!DeconzChecksum.IsValid(frame))
        {
            logger.LogWarning("Dropped a request with an invalid checksum ({@Length} bytes)", frame.Length);
            return;
        }

        var request = frame[..^ChecksumLength];
        if (request.Length == 0)
        {
            logger.LogWarning("Dropped an empty request");
            return;
        }

        var commandName = DeconzResponder.DescribeCommand(request[0]);
        var response = responder.HandleRequest(request);
        if (response.Length == 0)
        {
            logger.LogInformation("Received {@Command} request, no response to send", commandName);
            return;
        }

        logger.LogInformation(
            "Received {@Command} request, sending {@ResponseLength}-byte response",
            commandName,
            response.Length
        );
        await stream.WriteAsync(Framed(response), token);
    }

    private static byte[] Framed(byte[] response)
    {
        var checksum = DeconzChecksum.Compute(response);
        var withChecksum = new List<byte>(response) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
