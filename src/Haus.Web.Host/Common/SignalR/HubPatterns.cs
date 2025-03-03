using System.Linq;
using Haus.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Haus.Web.Host.Common.SignalR;

public static class HubPatterns
{
    private static readonly string[] All =
    [
        HausRealtimeSources.Diagnostics,
        HausRealtimeSources.DeviceSimulator,
        HausRealtimeSources.Events,
        HausRealtimeSources.Health,
    ];

    public static bool MatchesHub(PathString path)
    {
        return All.Any(pattern => path.StartsWithSegments(pattern));
    }
}
