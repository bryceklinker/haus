using Microsoft.Extensions.DependencyInjection;

namespace Haus.Web.Host;

public static class ServiceScopeExtensions
{
    public static T GetService<T>(this IServiceScope scope)
        where T : notnull
    {
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}
