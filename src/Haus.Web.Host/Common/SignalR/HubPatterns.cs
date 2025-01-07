using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Haus.Web.Host.Common.SignalR;

public static class HubPatterns
{
    public const string DiagnosticsHub = "/hubs/diagnostics";
    public const string DeviceSimulatorHub = "/hubs/device-simulator";
    public const string EventsHub = "/hubs/events";
    public const string HealthHub = "/hubs/health";

    private static readonly string[] All = [DiagnosticsHub, DeviceSimulatorHub, EventsHub, HealthHub];

    public static bool MatchesHub(PathString path)
    {
        return All.Any(pattern => path.StartsWithSegments(pattern));
    }
}