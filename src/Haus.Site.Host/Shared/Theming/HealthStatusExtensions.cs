using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MudBlazor;

namespace Haus.Site.Host.Shared.Theming;

public static class HealthStatusExtensions
{
    public static string ToColor(this HealthStatus? status, MudTheme? theme = null)
    {
        var actualTheme = theme ?? new HausTheme();
        return status switch
        {
            HealthStatus.Unhealthy => actualTheme.PaletteLight.Error.Value,
            HealthStatus.Degraded => actualTheme.PaletteLight.Warning.Value,
            HealthStatus.Healthy => actualTheme.PaletteLight.Success.Value,
            _ => actualTheme.PaletteLight.Info.Value,
        };
    }
}
