using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host;

public static class HostEnvironmentExtensions
{
    public static bool IsAcceptance(this IHostEnvironment env)
    {
        return env.IsEnvironment("Acceptance");
    }
}
