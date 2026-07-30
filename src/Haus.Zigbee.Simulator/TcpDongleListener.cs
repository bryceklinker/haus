using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Haus.Zigbee.Simulator;

public class TcpDongleListener(DeconzResponder responder, ILoggerFactory loggerFactory, int port) : BackgroundService
{
    private readonly ILogger<TcpDongleListener> _logger = loggerFactory.CreateLogger<TcpDongleListener>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        _logger.LogInformation("Fake deCONZ dongle listening on port {@Port}", port);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                _logger.LogInformation(
                    "Accepted dongle connection from {@RemoteEndPoint}",
                    client.Client.RemoteEndPoint
                );
                _ = HandleClientAsync(client, stoppingToken);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            listener.Stop();
        }
    }

    // Each connection runs detached from the accept loop so one slow/faulted connection never
    // blocks accepting the next; connection-level failures are logged inside DongleConnection.
    private async Task HandleClientAsync(TcpClient client, CancellationToken token)
    {
        var connectionLogger = loggerFactory.CreateLogger<DongleConnection>();
        await new DongleConnection(client, responder, connectionLogger).RunAsync(token);
    }
}
