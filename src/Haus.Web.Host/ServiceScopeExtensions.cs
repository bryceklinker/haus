using Microsoft.Extensions.DependencyInjection;

namespace Haus.Web.Host
{
    public static class ServiceScopeExtensions
    {
        public static T GetService<T>(this IServiceScope scope)
        {
            return scope.ServiceProvider.GetService<T>();
        }
    }
}