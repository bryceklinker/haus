using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Zigbee.Models;

namespace Haus.Acceptance.Tests.Support;

// Drives the fake deCONZ dongle (Haus.Zigbee.Simulator) over its HTTP control API to simulate a
// device pairing -- the deconz-cutover replacement for publishing a synthetic zigbee2mqtt
// interview_successful message. Unlike that old flow, the resulting ExternalId is always the
// device's IEEE address (Haus.Zigbee.Host derives it, it isn't chosen by the caller).
public class DeconzSimulatorClient(HttpClient httpClient)
{
    private const string PhilipsVendor = "Philips";
    private const string PhilipsLightModel = "929002335001";
    private const string PhilipsMotionSensorModel = "9290012607";

    public Task<string> JoinPhilipsLightAsync()
    {
        return JoinAsync(PhilipsVendor, PhilipsLightModel);
    }

    public Task<string> JoinPhilipsMotionSensorAsync()
    {
        return JoinAsync(PhilipsVendor, PhilipsMotionSensorModel);
    }

    private async Task<string> JoinAsync(string vendor, string model)
    {
        var ieeeAddress = new IeeeAddress((ulong)Random.Shared.NextInt64());
        var response = await httpClient.PostAsJsonAsync(
            "/devices/join",
            new
            {
                ieee = ieeeAddress.ToString(),
                vendor,
                model,
            }
        );
        response.EnsureSuccessStatusCode();
        return ieeeAddress.ToString();
    }
}
