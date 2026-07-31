using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Simulator.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Simulator;

[ApiController]
public class SimulatorController(DeconzResponder responder, IOptions<SimulatorOptions> options) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult GetStatus()
    {
        return Ok(new { status = "running", tcpPort = options.Value.TcpPort });
    }

    [HttpGet("/aps-requests")]
    public IActionResult GetApsRequests()
    {
        return Ok(responder.SentApsRequests.Select(request => Convert.ToHexString(request)));
    }

    [HttpPost("/parameters/{id}")]
    public IActionResult SetParameter([FromRoute] byte id, [FromBody] byte[] value)
    {
        responder.SetParameter(id, value);
        return NoContent();
    }

    [HttpPost("/indications")]
    public IActionResult EnqueueIndication([FromBody] IndicationRequest request)
    {
        byte[] asdu;
        try
        {
            asdu = Convert.FromHexString(request.AsduHex);
        }
        catch (FormatException)
        {
            return BadRequest("asduHex must be a valid hex string");
        }

        responder.EnqueueIndication(
            new IndicationBody(request.SourceNwk, request.SourceEndpoint, request.ProfileId, request.ClusterId, asdu)
        );
        return Accepted();
    }

    [HttpPost("/devices/join")]
    public async Task<IActionResult> JoinDevice([FromBody] JoinDeviceRequest request)
    {
        if (!IeeeAddress.TryParse(request.Ieee, out var ieeeAddress))
            return BadRequest("ieee must be a 0x-prefixed 16-hex-digit address");

        var networkAddress = responder.AllocateNetworkAddress();
        var baseIndex = DeviceJoinScenario.SimulateJoin(
            responder,
            ieeeAddress,
            networkAddress,
            request.Vendor,
            request.Model
        );
        var targetApsRequestCount = baseIndex + DeviceJoinScenario.ApsRequestsPerJoin;

        var completed = await WaitUntilAsync(
            () => responder.ApsRequestCount >= targetApsRequestCount,
            TimeSpan.FromSeconds(10)
        );

        return completed
            ? Ok(new { networkAddress })
            : Problem("Simulated device interview did not complete in time", statusCode: 504);
    }

    private static async Task<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        while (!condition() && !cts.IsCancellationRequested)
            await Task.Delay(20, CancellationToken.None);

        return condition();
    }
}

public record IndicationRequest(
    ushort SourceNwk,
    byte SourceEndpoint,
    ushort ProfileId,
    ushort ClusterId,
    string AsduHex
);

public record JoinDeviceRequest(string Ieee, string Vendor, string Model);
